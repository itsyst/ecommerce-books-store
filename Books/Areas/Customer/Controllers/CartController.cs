using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Books.Models;
using Books.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Books.Areas.Customer.Controllers
{
#pragma warning disable

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
                ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == userId, includeProperties: p => p.Product),
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
                includeProperties: p => p.Product),
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

            ShoppingCartViewModel.ShoppingCarts = await _shoppingCart.Entity.GetAllAsync(c => c.ApplicationUserId == userId, includeProperties: p => p.Product);

            // Shipping Details
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartViewModel.OrderHeader.ApplicationUserId = userId;
            ShoppingCartViewModel.OrderHeader.Company = appUser.Company;

            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = Status.Payment.Pending.ToString();
                ShoppingCartViewModel.OrderHeader.OrderStatus = Status.StatusType.Pending.ToString();
            }
            else
            {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = Status.Payment.Delayed.ToString();
                ShoppingCartViewModel.OrderHeader.OrderStatus = Status.StatusType.Approved.ToString();
            }


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

            // Stripe settings
            if (appUser.CompanyId.GetValueOrDefault() != 0)
                return RedirectToAction("OrderConfirmed", "Cart", new { id = ShoppingCartViewModel.OrderHeader.Id });

            //stripe settings 
            var domain = "https://localhost:44376/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
            {
                "card",
            },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmed?id={ShoppingCartViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in ShoppingCartViewModel.ShoppingCarts)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.SetPriceHolderRabat(item.Count * item.Product.Price * Rabat.DISCOUNT) * 100), //20.00 -> 2000
                        Currency = "sek",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title,
                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Stripe.Checkout.Session session = service.Create(options);

            //Update orderheader table.
            ShoppingCartViewModel.OrderHeader.SessionId = session.Id;
            ShoppingCartViewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            await _orderHeader.Entity.UpdateAsync(ShoppingCartViewModel.OrderHeader);
            await _orderHeader.CompleteAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public async Task<IActionResult> OrderConfirmed(int id)
        {
            var orderHeaderInDb = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "ApplicationUser");

            if (!orderHeaderInDb.PaymentStatus.Equals(Status.Payment.Delayed.ToString()))
            {
                var service = new SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeaderInDb.SessionId);

                // Check stripe status.
                if (session.PaymentStatus.ToLower().Equals("paid"))
                {
                    orderHeaderInDb.OrderStatus = Status.StatusType.Approved.ToString();
                    orderHeaderInDb.PaymentStatus = Status.Payment.Approved.ToString();
                    orderHeaderInDb.PaymentDate = DateTime.Now;

                    await _orderHeader.Entity.UpdateAsync(orderHeaderInDb);
                    await _orderHeader.CompleteAsync();
                }
            }

            // Retreive shoppingcarts from the database.
            var shoppingCartsInDb = await _shoppingCart.Entity.GetAllAsync(u => u.ApplicationUserId == orderHeaderInDb.ApplicationUserId, includeProperties: p => p.Product);

            // Remove shopping carts from database.
            await _shoppingCart.Entity.DeleteRangeAsync(shoppingCartsInDb);
            await _shoppingCart.CompleteAsync();

            return View(id);
        }
    }
}
