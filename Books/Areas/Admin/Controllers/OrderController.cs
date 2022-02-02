using Books.Domain.Entities;
using Books.Domain.ViewModels;
using AutoMapper;
using Books.Interfaces;
using Books.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Books.Utilities;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;

namespace Books.Areas.Admin.Controllers
{
#pragma warning disable

    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork<OrderHeader> _orderHeader;
        private readonly IUnitOfWork<OrderDetail> _orderDetail;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;
        private readonly IUnitOfWork<Domain.Entities.Product> _product;
        private readonly IMapper _mapper;


        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(
            IUnitOfWork<OrderHeader> orderHeader,
            IUnitOfWork<OrderDetail> orderDetail,
            IUnitOfWork<ApplicationUser> applicationUser,
            IUnitOfWork<Domain.Entities.Product> product,
            IMapper mapper)
        {
            _orderHeader = orderHeader;
            _orderDetail = orderDetail;
            _applicationUser = applicationUser;
            _product = product;
            _mapper = mapper;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }

        // GET: Order/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            OrderViewModel = new()
            {
                OrderHeader = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "ApplicationUser"),
                OrderDetail = await _orderDetail.Entity.GetAllAsync(filter: o => o.OrderId == id, p => p.Product, h => h.OrderHeader),
            };


            if (OrderViewModel == null)
            {
                return NotFound();
            }
            return View(OrderViewModel);
        }

        // GET: Order/Details/
        [HttpPost]
        [ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayNow()
        {
            OrderViewModel.OrderHeader = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == OrderViewModel.OrderHeader.Id, includeProperties: "ApplicationUser");
           
            if (OrderViewModel.OrderHeader == null)
                return NotFound();

            OrderViewModel.OrderDetail = await _orderDetail.Entity.GetAllAsync(filter: o => o.OrderId == OrderViewModel.OrderHeader.Id, p => p.Product, h => h.OrderHeader);

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
                SuccessUrl = domain + $"admin/order/PaymentConfirmed?id={OrderViewModel.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?id={OrderViewModel.OrderHeader.Id}",
            };

            foreach (var item in OrderViewModel.OrderDetail)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), //10.00 -> 1000
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
            OrderViewModel.OrderHeader.SessionId = session.Id;
            OrderViewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            await _orderHeader.Entity.UpdateAsync(OrderViewModel.OrderHeader);
            await _orderHeader.CompleteAsync();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
         }

        public async Task<IActionResult> PaymentConfirmed(int id)
        {
            var orderHeaderInDb = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == id, includeProperties: "ApplicationUser");

            if (orderHeaderInDb.PaymentStatus.Equals(Status.Payment.Delayed.ToString()))
            {
                var service = new SessionService();
                Stripe.Checkout.Session session = service.Get(orderHeaderInDb.SessionId);

                // Check stripe status.
                if (session.PaymentStatus.ToLower().Equals("paid"))
                {
                    orderHeaderInDb.OrderStatus = Status.StatusType.Approved.ToString();
                    orderHeaderInDb.PaymentStatus = Status.Payment.Approved.ToString();

                    await _orderHeader.Entity.UpdateAsync(orderHeaderInDb);
                    await _orderHeader.CompleteAsync();
                }
            }

            return View(id);
        }

        // POST: Order/Update
        [HttpPost]
        [Authorize(Roles = "Admin , Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderHeaderInDB = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == OrderViewModel.OrderHeader.Id, tracked: false);

            if (orderHeaderInDB == null)
                return NotFound();

            orderHeaderInDB.FirstName = OrderViewModel.OrderHeader.FirstName;
            orderHeaderInDB.LastName = OrderViewModel.OrderHeader.LastName;
            orderHeaderInDB.PhoneNumber = OrderViewModel.OrderHeader.PhoneNumber;
            orderHeaderInDB.StreetAddress = OrderViewModel.OrderHeader.StreetAddress;
            orderHeaderInDB.City = OrderViewModel.OrderHeader.City;
            orderHeaderInDB.State = OrderViewModel.OrderHeader.State;
            orderHeaderInDB.PostalCode = OrderViewModel.OrderHeader.PostalCode;

            if (OrderViewModel.OrderHeader.Carrier != null)
            {
                orderHeaderInDB.Carrier = OrderViewModel.OrderHeader.Carrier;
            }
            if (OrderViewModel.OrderHeader.TrackingNumber != null)
            {
                orderHeaderInDB.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
            }

            await _orderHeader.Entity.UpdateAsync(orderHeaderInDB);
            await _orderHeader.CompleteAsync();

            TempData["Success"] = "Order updated successfully.";

            return RedirectToAction("Details", "Order", new { id = orderHeaderInDB.Id });
        }


        // POST: Order/Process
        [HttpPost]
        [Authorize(Roles = "Admin , Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderHeaderInDB = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == OrderViewModel.OrderHeader.Id, tracked: false);

            if (orderHeaderInDB == null)
                return NotFound();

            orderHeaderInDB.OrderStatus = Status.StatusType.InProgress.ToString();
   
            await _orderHeader.Entity.UpdateAsync(orderHeaderInDB);
            await _orderHeader.CompleteAsync();

            TempData["Success"] = "Order status updated successfully.";

            return RedirectToAction("Details", "Order", new { id = orderHeaderInDB.Id });
        }

        // POST: Order/Process
        [HttpPost]
        [Authorize(Roles = "Admin , Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ship()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderHeaderInDB = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == OrderViewModel.OrderHeader.Id, tracked: false);

            if (orderHeaderInDB == null)
                return NotFound();

            orderHeaderInDB.TrackingNumber = OrderViewModel.OrderHeader.TrackingNumber;
            orderHeaderInDB.Carrier = OrderViewModel.OrderHeader.Carrier;
            orderHeaderInDB.OrderStatus = Status.StatusType.Shipped.ToString();
            orderHeaderInDB.ShippingDate = DateTime.Now;

            if (orderHeaderInDB.PaymentStatus.Equals(Status.Payment.Delayed))
                orderHeaderInDB.PaymentStatus = DateTime.Now.AddDays(ArrivalDate.Month).ToShortDateString();
         
            await _orderHeader.Entity.UpdateAsync(orderHeaderInDB);
            await _orderHeader.CompleteAsync();

            TempData["Success"] = "Order shipped successfully.";

            return RedirectToAction("Details", "Order", new { id = orderHeaderInDB.Id });
        }

        // POST: Order/Cancel
        [HttpPost]
        [Authorize(Roles = "Admin , Employee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel()
        {
            var orderHeaderInDB = await _orderHeader.Entity.GetFirstOrDefaultAsync(o => o.Id == OrderViewModel.OrderHeader.Id, tracked: false);

            if (orderHeaderInDB == null)
                return NotFound();

            if(orderHeaderInDB.PaymentStatus.Equals(Status.Payment.Approved.ToString()))
            {
                // Using stripe.
                var options = new RefundCreateOptions()
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderInDB.PaymentIntentId,
                };

                var service = new RefundService();
                Refund refund = await service.CreateAsync(options);

                // Update status.
                orderHeaderInDB.OrderStatus = Status.StatusType.Canceled.ToString();
                orderHeaderInDB.PaymentStatus = Status.StatusType.Refunded.ToString();
            }
            else
            {
                orderHeaderInDB.OrderStatus = Status.StatusType.Canceled.ToString();
                orderHeaderInDB.PaymentStatus = Status.StatusType.Canceled.ToString();
            }

            // Recalculate product count in stock
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orderDetailInDB = await _orderDetail.Entity.GetFirstOrDefaultAsync(o => o.OrderId == orderHeaderInDB.Id, includeProperties: "Product");
            var productInDB = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == orderDetailInDB.ProductId);

            productInDB.InStock += orderDetailInDB.Count;
 
            await _product.Entity.UpdateAsync(productInDB);
            await _product.CompleteAsync();

            await _orderHeader.Entity.UpdateAsync(orderHeaderInDB);
            await _orderHeader.CompleteAsync();

            TempData["Success"] = "Order canceled and refunded successfully.";

            return RedirectToAction("Details", "Order", new { id = orderHeaderInDB.Id });
        }
    }
}
