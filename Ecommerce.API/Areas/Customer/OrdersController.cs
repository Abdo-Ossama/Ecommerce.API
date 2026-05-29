using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Ecommerce.API.Areas.Customer
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER)]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;

        public OrdersController(UserManager<ApplicationUser> userManager,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll(int? id, int page = 1)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();


            var ordersUser = await _orderRepository.GetAsync(e => e.ApplicationUser.Id == userId);

            //Filter with Name..
            if (id is not null)
            { 
                ordersUser = await _orderRepository.GetAsync(e => e.Id == id);
            }


            // Pagination
            int currentPage = page;
            int pageSize = 5;
            // Total Pages

            {

                double totalPages = Math.Ceiling((double)ordersUser.Count() / pageSize);
                if (page < 1)
                {
                    page = 1;
                }
                var brands = ordersUser
                            .Skip((currentPage - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                return Ok(new 
                {
                    Orders = ordersUser,
                    totalPages = totalPages,
                    pageSize = pageSize

                });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var ordersUser = await _orderRepository.GetOneAsync(e => e.ApplicationUser.Id == userId);


            var order = await _orderItemRepository.GetAsync(e => e.Id == id);

            return Ok(order);
        }

        [HttpGet("{id}/Refund")]
        public async Task<IActionResult> Refund(int id)
        {
            var order = await _orderRepository.GetOneAsync(e => e.Id == id);

            if (order is null) return NotFound();

            if (order.PaymentStatus == PaymentStatus.Refunded || order.OrderStatus == OrderStatus.Canceled)
                return BadRequest();

            var options = new RefundCreateOptions()
            {
                Reason = RefundReasons.Unknown,
                Amount = ((long)order.TotalPrice * 100) - (5 * 100),
                PaymentIntent = order.TransactionId
            };

            var service = new RefundService();
            var session = service.Create(options);

            order.OrderStatus = OrderStatus.Canceled;
            order.PaymentStatus = PaymentStatus.Refunded;
            await _orderRepository.CommitAsync();

            return NoContent();
        }
    }
}
