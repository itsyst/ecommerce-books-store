using BooksWebApp.Data;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _db;

        public CategoryController(ApplicationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _db.Categories;
            return View(categoryList);
        }

        // Get
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            try
            {
                if (category.Name == category.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("OrderError", "The order can not match the Name.");
                }

                if (ModelState.IsValid)
                {
                    var obj = _db.Categories.Single(c => c.Name == category.Name);
                    if (obj == null)
                    {
                        _db.Categories.Add(category);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomerError", "This category already.");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "An Expected Error!");
            }

            return View(category);
        }
    }
}
