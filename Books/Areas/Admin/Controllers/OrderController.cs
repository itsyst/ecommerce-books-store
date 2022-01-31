using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Books.Models;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers
{
#pragma warning disable

    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork<OrderHeader> _orderHeader;
        private readonly IUnitOfWork<OrderDetail> _orderDetail;
        private readonly IUnitOfWork<ApplicationUser> _applicationUser;

        [BindProperty]
        public OrderViewModel OrderViewModel { get; set; }

        public OrderController(
            IUnitOfWork<OrderHeader> orderHeader,
            IUnitOfWork<OrderDetail> orderDetail,
            IUnitOfWork<ApplicationUser> applicationUser)
        {
            _orderHeader = orderHeader;
            _orderDetail = orderDetail;
            _applicationUser = applicationUser;
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
                OrderDetail = await _orderDetail.Entity.GetAllAsync(filter: o => o.Id == id, p => p.Product, h => h.OrderHeader),
            };

 
            if (OrderViewModel == null)
            {
                return NotFound();
            }
            return View(OrderViewModel);
        }
    }
}
