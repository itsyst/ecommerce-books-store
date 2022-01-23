using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Books.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork<ShoppingCart> _shoppingCart;
        public CartController(IUnitOfWork<ShoppingCart> shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }
        // GET: CartController
        public async Task<ActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = new ShoppingCartViewModel()
            {
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(
                    c => c.ApplicationUserId == userId,
                   includeProperties: "Product"),


            }; 
            return View(cart);
        }

        protected override void Dispose(bool disposing)
        {
            _shoppingCart.Dispose();
            base.Dispose(disposing);
        }
    }
}
