using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
