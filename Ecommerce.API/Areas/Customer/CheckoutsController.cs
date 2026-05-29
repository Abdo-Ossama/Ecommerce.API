using Ecommerce.API.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace Ecommerce.API.Areas.Customer
{
    [Route("[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER)]
    [Authorize]
    public class CheckoutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly ILogger<CheckoutsController> _logger;
        private readonly ICartRepository _cartRepository;

        public CheckoutsController(IRepository<Order> orderRepository,
            ApplicationDbContext context,
            ILogger<CheckoutsController> logger,
            ICartRepository cartRepository,
            IRepository<OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _context = context;
            _logger = logger;
            _cartRepository = cartRepository;
            _orderItemRepository = orderItemRepository;
        }

        [HttpGet("{orderId}/Success")]
        public async Task<IActionResult> Success(int orderId)
        {
            var transaction = _context.Database.BeginTransaction();

            try
            {
                var order = await _orderRepository.GetOneAsync(e => e.Id == orderId,
                    include: [e => e.ApplicationUser]);

                if (order is null) return NotFound();

                // Update Transaction Id
                var service = new SessionService();
                var session = service.Get(order.SessionId);

                order.TransactionId = session.PaymentIntentId;
                order.OrderStatus = OrderStatus.InProcessing;
                order.PaymentStatus = PaymentStatus.Successed;

                await _orderRepository.CommitAsync();

                // Move Cart => Order Item
                var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == order.ApplicationUserId,
                include: [e => e.Product]);

                foreach (var item in userCart)
                {
                    await _orderItemRepository.CreateAsync(new()
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Count = item.Count,
                        PricePerProduct = item.PricePerProduct,
                    });
                }
                await _orderItemRepository.CommitAsync();

                // Delete Old Cart
                _cartRepository.DeleteRange(userCart);
                await _cartRepository.CommitAsync();


                transaction.Commit();

                return Ok(new PaymentSuccessResponse()
                {
                    OrderDate = order.CreateAt,
                    PaymentMethod = order.PaymentMethod.ToString(),
                    TotalPrice = order.TotalPrice,
                    OrderId = order.Id,
                    Email = order.ApplicationUser.Email!
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                transaction.Rollback();
                return BadRequest();
            }
        }
    }
}
