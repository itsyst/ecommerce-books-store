using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Areas.Admin.Controllers
{
#pragma warning disable CS8604
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
            return View(await Task.FromResult(_cover.Entity.GetAll()));
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
                    _cover.Entity.Insert(cover);
                    await _cover.SaveAsync();
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

            var cover = _cover.Entity.GetFirstOrDefault(c => c.Id == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(cover));
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
                    _cover.Entity.Update(cover);
                    await _cover.SaveAsync();
                    TempData["Success"] = "Category upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoverExists(cover.Id))
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

            var cover = _cover.Entity.GetFirstOrDefault(c => c.Id == id);
            if (cover == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(cover));
        }

        // POST: CoverController/Delete/5
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var cover = _cover.Entity.GetById(id);
                _cover.Entity.Delete(cover.Id);
                await _cover.SaveAsync();
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private bool CoverExists(int id)
        {
            return _cover.Entity.Exists(id);
        }
    }
}
