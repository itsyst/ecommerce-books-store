using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Books.Utilities;
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
                   includeProperties: "Product")

            };

            foreach (var item in cart.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                cart.TotalSum += (item.PriceHolderRabat());
            }

            return View(cart);
        }

    }
}
