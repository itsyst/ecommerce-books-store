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
#pragma warning disable CS8618  

    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork<Product> _product;
        private readonly IUnitOfWork<ShoppingCart> _shoppingCart;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<OrderHeader> _orderHeader;
        private readonly IUnitOfWork<OrderDetail> _orderDetail;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public CartController(
            IUnitOfWork<Product> product,
            IUnitOfWork<ShoppingCart> shoppingCart,
            IUnitOfWork<ApplicationUser> applicationUser,
            IUnitOfWork<OrderHeader> orderHeader,
            IUnitOfWork<OrderDetail> orderDetail)
        {
            _product = product;
            _shoppingCart = shoppingCart;
            _applicationUser = applicationUser;
            _orderHeader = orderHeader;
            _orderDetail = orderDetail;
        }

        // GET: CartController
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == userId, includeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var item in ShoppingCartViewModel.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (item.PriceHolderRabat());
            }

            return View(ShoppingCartViewModel);
        }

        /// <summary>
        /// Add new item to the shopping basket.
        /// </summary>
        /// <param name="cartId">Shopping cart Id</param>
        /// <returns></returns>
        public async Task<IActionResult> AddItem(Guid? cartId)
        {
            ShoppingCart? cart = await _shoppingCart.Entity.GetFirstOrDefaultAsync(u => u.Id == cartId, includeProperties: "Product");

            if (cart != null)
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

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(
                c => c.ApplicationUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartViewModel.OrderHeader.ApplicationUser = await _applicationUser.Entity.GetFirstOrDefaultAsync(u => u.Id == userId);
            ShoppingCartViewModel.OrderHeader.FirstName = ShoppingCartViewModel.OrderHeader.ApplicationUser.FirstName;
            ShoppingCartViewModel.OrderHeader.LastName = ShoppingCartViewModel.OrderHeader.ApplicationUser.LastName;
            ShoppingCartViewModel.OrderHeader.PhoneNumber = ShoppingCartViewModel.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartViewModel.OrderHeader.StreetAddress = ShoppingCartViewModel.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartViewModel.OrderHeader.City = ShoppingCartViewModel.OrderHeader.ApplicationUser.City;
            ShoppingCartViewModel.OrderHeader.State = ShoppingCartViewModel.OrderHeader.ApplicationUser.State;
            ShoppingCartViewModel.OrderHeader.PostalCode = ShoppingCartViewModel.OrderHeader.ApplicationUser.PostalCode;
 
            foreach (var item in ShoppingCartViewModel.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (item.PriceHolderRabat());
            }

            if (!(ShoppingCartViewModel.OrderHeader.OrderTotal > 0))
                return RedirectToAction(actionName: "Index", controllerName: "Cart");

            return View(ShoppingCartViewModel);
        }


        /// <summary>
        /// Show Order summary.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SetSummary()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appUser = await _applicationUser.Entity.GetFirstOrDefaultAsync(a => a.Id == userId, includeProperties: "Company");

            //ShoppingCartViewModel.ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == userId,includeProperties: "Product");
            ShoppingCartViewModel.ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == userId, includeProperties: "Product");

            // Shipping Details
            ShoppingCartViewModel.OrderHeader.PaymentStatus = Status.Payment.Pending.ToString();
            ShoppingCartViewModel.OrderHeader.OrderStatus = Status.StatusType.Pending.ToString();
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now.ToString();
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
            ShoppingCartViewModel.OrderHeader.Company = appUser.Company;



            foreach (var item in ShoppingCartViewModel.ShoppingCarts)
            {
                item.PriceHolder = item.Count * item.Product.Price;
                item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT);
                ShoppingCartViewModel.OrderHeader.OrderTotal += (item.PriceHolderRabat());
            }

            await _orderHeader.Entity.InsertAsync(ShoppingCartViewModel.OrderHeader);
            await _orderHeader.CompleteAsync();


            // Order Summary
            foreach (var item in ShoppingCartViewModel.ShoppingCarts)
            {
                OrderDetail oderDetail = new()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = item.PriceHolderRabat(),
                    Count = item.Count
                };

                await _orderDetail.Entity.InsertAsync(oderDetail);
                await _orderDetail.CompleteAsync();
            }

            // Decrease product count in stock
            ShoppingCart? cart = await _shoppingCart.Entity.GetFirstOrDefaultAsync(u => u.ApplicationUserId == ShoppingCartViewModel.OrderHeader.ApplicationUserId, includeProperties: "Product");
            cart.Product.InStock -= ShoppingCartViewModel.ShoppingCarts.Count<ShoppingCart>();
            await _shoppingCart.Entity.UpdateAsync(cart);
            await _shoppingCart.CompleteAsync();

            // Remove shopping carts from database.
            await _shoppingCart.Entity.DeleteRangeAsync(ShoppingCartViewModel.ShoppingCarts);
            await _shoppingCart.CompleteAsync();

            return RedirectToAction(actionName: "Index", controllerName: "Home");

        }
    }
}
