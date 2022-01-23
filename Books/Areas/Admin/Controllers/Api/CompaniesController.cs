using AutoMapper;
using Books.Domain.Entities;
using Books.Dtos;
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
        private readonly IMapper _mapper;

        public CompaniesController(IUnitOfWork<Company> company, IMapper mapper)
        {
            _company = company;
            _mapper = mapper;
        }

        // GET: /api/companies
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await _company.Entity.GetAllAsync();
            return Ok(companies);
        }

        //GET: /api/v1/companies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {

            var company = await _company.Entity.GetFirstOrDefaultAsync(p => p.Id == id);

            if (company == null)
                return NotFound();

            return Ok(_mapper.Map<Company, CompanyDto>(company));
        }


        // PUT /api/products/1
        [HttpPut]
        public async Task<IActionResult> UpsertProduct(int id, CompanyDto companyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var companyInDb = await _company.Entity.GetFirstOrDefaultAsync(p => p.Id == id);

            if (companyInDb == null)
                return NotFound();

            var _mappedCompany = _mapper.Map(companyDto, companyInDb);

            await _company.Entity.UpdateAsync(_mappedCompany);
            await _company.CompleteAsync();

            return Ok();
        }


        // GET: /api/companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            Company companyInDb = await _company.Entity.GetFirstOrDefaultAsync(c => c.Id == id);

            if (companyInDb == null)
                return NotFound();

            await _company.Entity.DeleteAsync(companyInDb.Id);
            await _company.CompleteAsync();

            return Ok(companyInDb);

        }
    }
}
