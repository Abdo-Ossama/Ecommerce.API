using Azure.Core;
using Ecommerce.API.DTOs.Requests;
using Ecommerce.API.DTOs.Responses;
using Ecommerce526.API.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.API.Areas.Customer
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area(SD.CUSTOMER)]
    [Authorize]
    public class CartsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartRepository _cartRepository;
        private readonly IRepository<Models.Product> _productRepository;
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Order> _orderRepository;

        public CartsController(UserManager<ApplicationUser> userManager, ICartRepository cartRepository, IRepository<Models.Product> productRepository, IRepository<Promotion> promotionRepository, IRepository<Order> orderRepository)
        {
            _userManager = userManager;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _promotionRepository = promotionRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string? promotionCode = null, CancellationToken cancellationToken = default)

        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();


            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == user.Id,
                include: [e => e.Product]
               );

            string error_notification = string.Empty;
            string success_notification = string.Empty;

            if (promotionCode is not null)
            {
                // Check if promotionCode is exist & is valid
                var promotion = await _promotionRepository.GetOneAsync(e => e.Code == promotionCode && e.Usage > 0);
                if (promotion is null)
                {
                    error_notification = "Invalid promotion";

                    return BadRequest(new ErrorResponse()
                    {
                        Message = error_notification
                    });
                }

                // Check product list in cart, matching product list in promotion code
                if (promotion.ProductId is null)
                {
                    // Apply discount
                    var cartTotal = userCart.Sum(e => e.ListPrice);
                    var discount = cartTotal - (userCart.Sum(e => e.ListPrice) * promotion.Discount / 100);

                    //
                }
                else
                {
                    //userCart.Select(e => e.ProductId).ToList().Contains(promotion.ProductId);

                    foreach (var item in userCart)
                    {
                        if (item.ProductId == promotion.ProductId)
                        {
                            var cartTotal = item.ListPrice;
                            var applyDiscount = cartTotal - (item.ListPrice * promotion.Discount / 100);

                            item.PricePerProduct = applyDiscount;
                            item.ListPrice = item.PricePerProduct * item.Count;
                            await _cartRepository.CommitAsync();

                            success_notification = "Apply Code Successfully";
                        }
                    }

                    if (promotion is null)
                    {
                        error_notification = "Can not apply this promotion code on the product in the current list";
                    }
                }
            }

            if (error_notification != string.Empty)
            {
                return BadRequest(new ErrorResponse()
                {
                 Message = error_notification
                });
            }
            else
            {
                return Ok(new CartResponse()
                {
                    Carts = userCart,
                    Message = success_notification
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(CartCreateRequest cartCreateRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var product = await _productRepository.GetOneAsync(e => e.Id == cartCreateRequest.productId);
            if (product is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == cartCreateRequest.productId && e.ApplicationUserId == user.Id);

            if (cart is null)
            {
                await _cartRepository.CreateAsync(new()
                {
                    ApplicationUserId = user.Id,
                    ProductId = cartCreateRequest.productId,
                    Count = cartCreateRequest.count,
                    PricePerProduct = product.Price,
                    ListPrice = product.Price * cartCreateRequest.count
                });
            }
            else
            {
                cart.Count += cartCreateRequest.count;
                cart.PricePerProduct = product.Price;
                cart.ListPrice = product.Price * cartCreateRequest.count;
            }

            await _cartRepository.CommitAsync();

            string success_notification = "Add Product Successfully to the cart";

            return Ok(new SuccessResponse<bool>
            {

                Msg = success_notification,
                Data = true
            }); 
        }

        [HttpPatch("{productId}/IncrementCount")]
        public async Task<IActionResult> IncrementCount(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            cart.Count += 1;
            await _cartRepository.CommitAsync();

            return NoContent();
        }

        [HttpPatch("{productId}/DecrementCount")]
        public async Task<IActionResult> DecrementCount(int productId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            if (cart.Count > 1)
            {
                cart.Count -= 1;
                await _cartRepository.CommitAsync();
            }

            return NoContent();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(int productId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var cart = await _cartRepository.GetOneAsync(e => e.ProductId == productId && e.ApplicationUserId == user.Id);

            if (cart is null) return NotFound();

            _cartRepository.delete(cart);
            await _cartRepository.CommitAsync();

            return NoContent();
        }

        [HttpGet("Pay")]
        public async Task<IActionResult> Pay()
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           if (userId is null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var userCart = await _cartRepository
                .GetAsync(e => e.ApplicationUserId == user.Id,
                include: [e => e.Product]);

            Order order = new()
            {
                ApplicationUserId = user.Id,
                TotalPrice = userCart.Sum(e => e.ListPrice)
            };
            await _orderRepository.CreateAsync(order);
            await _orderRepository.CommitAsync();

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/success?orderId={order.Id}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/customer/checkout/cancel?orderId={order.Id}",
                LineItems = new List<SessionLineItemOptions>()
            };
            foreach (var item in userCart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    Quantity = item.Count,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long)(item.PricePerProduct * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description
                        }
                    }
                });
            }

            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);

            order.SessionId = session.Id;
          await _orderRepository.CommitAsync();
            return Ok(session.Url);
        }
    }
}