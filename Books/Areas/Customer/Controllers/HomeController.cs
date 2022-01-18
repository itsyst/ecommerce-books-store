using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Books.Controllers
{
# pragma warning disable IDE0052
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork<Product> _product;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IUnitOfWork<Product> product, ILogger<HomeController> logger)
        {
            _product = product;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var product = await _product.Entity.GetAllAsync(includeProperties: "Category,Author,Cover");
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}