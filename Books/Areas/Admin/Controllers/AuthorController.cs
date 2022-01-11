using Books.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Books.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
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
            IEnumerable <Author> authors = _author.Entity.GetAll(includeProperties: "Products").ToList();
            return View(await Task.FromResult(authors));
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
                    var obj = _author.Entity.GetFirstOrDefault(c => c.FullName == author.FullName);
                    if (obj == null)
                    {
                        _author.Entity.Insert(author);
                        await _author.SaveAsync();
                        TempData["Success"] = "Author created successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("AuthorError", "This author already exists.");
                    }
                }
            }
            catch
            {
                ModelState.AddModelError("Error", "An Expected Error!");
            }

            return View(author);
        }

 
        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Author autor = _author.Entity.GetFirstOrDefault(a => a.Id == id, includeProperties: "Products");

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

            var category = _author.Entity.GetFirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(await Task.FromResult(category));
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
                    _author.Entity.Update(author);
                    await _author.SaveAsync();
                    TempData["Success"] = "Category upaded successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
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

            Author author = _author.Entity.GetFirstOrDefault(c => c.Id == id);
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
            var author = _author.Entity.GetById(id);
            _author.Entity.Delete(author.Id);
            await _author.SaveAsync();
            TempData["Success"] = "Author deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _author.Entity.Exists(id);
        }
    }
}
