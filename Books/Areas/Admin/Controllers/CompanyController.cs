using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork<Company> _company;

        public CompanyController(IUnitOfWork<Company> company)
        {
            _company = company;
        }



        // GET: CompanyController
        public async Task<IActionResult> Index()
        {
            IEnumerable<Company> companies = await _company.Entity.GetAllAsync();
            return View(companies);
        }

        // GET: ProductController/Upsert/5
        public async Task<IActionResult> Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
                return View(company);
            else
            {
                company = await _company.Entity.GetFirstOrDefaultAsync(u => u.Id == id);
                return View(company);
            }
        }

        // POST: ProductController/Upsert/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Company company)
        {
            if (ModelState.IsValid)
            {


                if (company.Id == 0)
                    await _company.Entity.InsertAsync(company);

                else
                {
                    await _company.Entity.UpdateAsync(company);
                }

                await _company.CompleteAsync();
                TempData["Success"] = "Company saved successfully.";

                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
    }
}
