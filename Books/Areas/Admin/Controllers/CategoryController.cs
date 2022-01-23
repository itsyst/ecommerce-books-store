using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
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
            IEnumerable<Category> categories = await _category.Entity.GetAllAsync();
            return View(categories);
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
        public async Task<IActionResult> Create([Bind("Id,Name,DisplayOrder")] Category category)
        {
            try
            {
                if (category.Name == category.DisplayOrder.ToString())
                {
                    ModelState.AddModelError("OrderError", "The order can not match the Name.");
                }

                if (ModelState.IsValid)
                {
                    var obj = await _category.Entity.GetFirstOrDefaultAsync(c => c.Id == category.Id);
                    if (obj == null)
                    {
                        await _category.Entity.InsertAsync(category);
                        await _category.CompleteAsync();
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

            var category = await _category.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
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
                    await _category.Entity.UpdateAsync(category);
                    await _category.CompleteAsync();
                    TempData["Success"] = "Category upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await CategoryExists(category.Id))
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

            var category = await _category.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
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
            var category = await _category.Entity.GetByIdAsync(id);
            await _category.Entity.DeleteAsync(category.Id);
            await _category.CompleteAsync();
            TempData["Success"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CategoryExists(int id)
        {
            return await _category.Entity.ExistsAsync(id);
        }
    }
}
