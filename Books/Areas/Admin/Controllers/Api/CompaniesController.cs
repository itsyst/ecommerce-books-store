using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers.Api
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class CompaniesController : ControllerBase
    {
        private readonly IUnitOfWork<Company> _company;
 
        public CompaniesController(IUnitOfWork<Company> company)
        {
            _company = company;
        }

        // GET: / api / companies
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _company.Entity.GetAllAsync();
            return Ok(companies);
        }
    }
}
