using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork<OrderHeader> _orderHeader;
        private readonly IUnitOfWork<OrderDetail> _orderDetail;

        public OrderController(
            IUnitOfWork<OrderHeader> orderHeader,
            IUnitOfWork<OrderDetail> orderDetail)
        {
            _orderHeader = orderHeader;
            _orderDetail = orderDetail;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            await Task.CompletedTask;
            return View();
        }
    }
}
