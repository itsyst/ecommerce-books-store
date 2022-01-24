using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Books.Models;
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
        private readonly IUnitOfWork<Product> _product;
        private readonly IUnitOfWork<ShoppingCart> _shoppingCart;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;

        public CartController(
            IUnitOfWork<Product> product, 
            IUnitOfWork<ShoppingCart> shoppingCart,
            IUnitOfWork<ApplicationUser> applicationUser)
        {
            _product = product;
            _shoppingCart = shoppingCart;
            _applicationUser = applicationUser;
        }

        // GET: CartController
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = new ShoppingCartViewModel()
            {
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(
                    c => c.ApplicationUserId == userId,
                   includeProperties: "Product"),
                OrderHeader = new()

            };

            foreach (var item in cart.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                cart.OrderHeader.OrderTotal += (item.PriceHolderRabat());
            }

            return View(cart);
        }

        /// <summary>
        /// Add new item to the shopping basket.
        /// </summary>
        /// <param name="cartId">Shopping cart Id</param>
        /// <returns></returns>
        public async Task<IActionResult> AddItem(Guid? cartId)
        {
            ShoppingCart? cart = await _shoppingCart.Entity.GetFirstOrDefaultAsync(u => u.Id == cartId, includeProperties: "Product");

            if(cart != null)
            {
                if (cart.Product.InStock > 0)
                {
                    cart.Count += 1;
                    await _shoppingCart.Entity.UpdateAsync(cart);
                    await _shoppingCart.CompleteAsync();

                    cart.Product.InStock -= 1;
                    await _product.Entity.UpdateAsync(cart.Product);
                    await _product.CompleteAsync();
                }
                else
                {
                    TempData["Error"] = "Not enough items In stock.";
                }
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Remove items from the shopping basket.
        /// </summary>
        /// <param name="cartId">Shopping cart Id</param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveItem(Guid? cartId)
        {
            ShoppingCart? cart = await _shoppingCart.Entity.GetFirstOrDefaultAsync(u => u.Id == cartId, includeProperties: "Product");

            if (cart != null)
            {
                if (cart.Count > 0)
                {
                    cart.Count -= 1;
                    await _shoppingCart.Entity.UpdateAsync(cart);
                    await _shoppingCart.CompleteAsync();

                    cart.Product.InStock += 1;
                    await _product.Entity.UpdateAsync(cart.Product);
                    await _product.CompleteAsync();
                }

            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Remove item from the shopping cart.
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveFromBasket(Guid cartId)
        {
            ShoppingCart? cart = await _shoppingCart.Entity.GetFirstOrDefaultAsync(u => u.Id == cartId, includeProperties: "Product");
            var items = cart.Count;

            if (cart != null)
            {
                cart.Count -= items;
                await _shoppingCart.Entity.UpdateAsync(cart);
                await _shoppingCart.CompleteAsync();

                cart.Product.InStock += items;
                await _product.Entity.UpdateAsync(cart.Product);
                await _product.CompleteAsync();
            }

            await _shoppingCart.Entity.DeleteAsync(cartId);
            await _shoppingCart.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        
        /// <summary>
        /// Show Order summary.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Summary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cart = new ShoppingCartViewModel()
            {
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(
                c => c.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

 
            cart.OrderHeader.ApplicationUser = await _applicationUser.Entity.GetFirstOrDefaultAsync(u => u.Id == userId);

            cart.OrderHeader.FirstName = cart.OrderHeader.ApplicationUser.FirstName;
            cart.OrderHeader.LastName = cart.OrderHeader.ApplicationUser.LastName;
            cart.OrderHeader.PhoneNumber = cart.OrderHeader.ApplicationUser.PhoneNumber;
            cart.OrderHeader.StreetAddress = cart.OrderHeader.ApplicationUser.StreetAddress;
            cart.OrderHeader.City = cart.OrderHeader.ApplicationUser.City;
            cart.OrderHeader.State = cart.OrderHeader.ApplicationUser.State;
            cart.OrderHeader.PostalCode = cart.OrderHeader.ApplicationUser.PostalCode;

            foreach (var item in cart.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                cart.OrderHeader.OrderTotal += (item.PriceHolderRabat());
            }

            if (!(cart.OrderHeader.OrderTotal > 0))
                return RedirectToAction(actionName: "Index", controllerName:"Cart");

            return View(cart);
        }
    }
}
