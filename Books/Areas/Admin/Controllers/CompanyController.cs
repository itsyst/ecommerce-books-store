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
    }
}
