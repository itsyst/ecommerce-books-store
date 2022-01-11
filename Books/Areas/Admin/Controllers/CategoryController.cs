using Books.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Books.Interfaces;

namespace Books.Controllers
{
#pragma warning disable CS8604
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork<Category> _category;

        public CategoryController(IUnitOfWork<Category> category)
        {
            _category = category;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            IEnumerable <Category> categories = _category.Entity.GetAll();
            return View(await Task.FromResult(categories));
        }

        // GET: Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create  
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for   
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DisplayOrder,CreatedDateTime")] Category category)
        {
            try
            {
                if (category.Name == category.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("OrderError", "The order can not match the Name.");
                }

                if (ModelState.IsValid)
                {
                    var obj = _category.Entity.GetFirstOrDefault(c => c.Name == category.Name);
                    if (obj == null)
                    {
                        _category.Entity.Insert(category);
                        await _category.SaveAsync();
                        TempData["Success"] = "Category created successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("CustomerError", "This category already exists.");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("", "An Expected Error!");
            }

            return View(category);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = _category.Entity.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(category));
        }

        // POST: Employee/Edit/5  
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for   
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DisplayOrder,CreatedDateTime")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _category.Entity.Update(category);
                    await _category.SaveAsync();
                    TempData["Success"] = "Category upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Employee/Delete/5  
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _category.Entity.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(category));
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = _category.Entity.GetById(id);
            _category.Entity.Delete(category.Id);
            await _category.SaveAsync();
            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _category.Entity.Exists(id);
        }
    }
}
