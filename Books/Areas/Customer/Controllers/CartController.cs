using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        // GET: CartController
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}
