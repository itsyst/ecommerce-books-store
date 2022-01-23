using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverController : Controller
    {
        private readonly IUnitOfWork<Cover> _cover;

        public CoverController(IUnitOfWork<Cover> cover)
        {
            _cover = cover;
        }

        // GET: CoverController
        public async Task<ActionResult> Index()
        {
            IEnumerable<Cover> covers = await _cover.Entity.GetAllAsync();
            return View(covers);
        }

        // GET: CoverController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoverController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Cover cover)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _cover.Entity.InsertAsync(cover);
                    await _cover.CompleteAsync();
                    TempData["Success"] = "Cover created successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An Expected Error!");
            }

            return View();
        }

        // GET: CoverController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var cover = await _cover.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(cover);
        }

        // POST: CoverController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Cover cover)
        {
            if (id != cover.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _cover.Entity.UpdateAsync(cover);
                    await _cover.CompleteAsync();
                    TempData["Success"] = "Category upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await CoverExists(cover.Id))
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
            return View(cover);
        }


        // GET: CoverController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cover = await _cover.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(cover);
        }

        // POST: CoverController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cover = await _cover.Entity.GetByIdAsync(id);
                await _cover.Entity.DeleteAsync(cover.Id);
                await _cover.CompleteAsync();
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private async Task<bool> CoverExists(int id)
        {
            return await _cover.Entity.ExistsAsync(id);
        }
    }
}
