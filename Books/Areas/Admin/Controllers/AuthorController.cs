using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
#pragma warning disable

    [Area("Admin")]
    public class AuthorController : Controller
    {
        private readonly IUnitOfWork<Author> _author;

        public AuthorController(IUnitOfWork<Author> author)
        {
            _author = author;
        }

        // GET: Author
        public async Task<IActionResult> Index()
        {
            var authors = await _author.Entity.GetAllAsync(filter: null, includeProperties: a=>a.Products);
            return View(authors);
        }

        // GET: Author/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author/Create  
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for   
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName")] Author author)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var obj = await _author.Entity.GetFirstOrDefaultAsync(c => c.Id == author.Id);
                    if (obj == null)
                    {
                        await _author.Entity.InsertAsync(author);
                        await _author.CompleteAsync();
                        TempData["Success"] = "Author created successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "An unexpected error occurred!");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("Error", "An Expected Error!");
            }

            return View(author);
        }


        // GET: Author/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author autor = await _author.Entity.GetFirstOrDefaultAsync(a => a.Id == id, includeProperties: "Products");

            if (autor == null)
            {
                return NotFound();
            }
            return View(await Task.FromResult(autor));
        }

        // GET: Author/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var author = await _author.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(author));
        }

        // POST: Author/Edit/5  
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for   
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _author.Entity.UpdateAsync(author);
                    await _author.CompleteAsync();
                    TempData["Success"] = "author upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await AuthorExists(author.Id))
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
            return View(author);
        }

        // GET: Author/Delete/5  
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author author = await _author.Entity.GetFirstOrDefaultAsync(c => c.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(author));
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _author.Entity.GetByIdAsync(id);
            await _author.Entity.DeleteAsync(author.Id);
            await _author.CompleteAsync();
            TempData["Success"] = "Author deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _author.Entity.ExistsAsync(id);
        }
    }
}
