using AutoMapper;
using Books.Domain.Entities;
using Books.Dtos;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers.Api
{
#pragma warning disable

    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork<Product> _product;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork<Product> product, IWebHostEnvironment hostEnvironment, IMapper mapper)
        {
            _product = product;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
        }

        // GET: / api / products
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _product.Entity.GetAllAsync(filter: null, c=>c.Category,a=>a.Author,v=>v.Cover);
            return Ok(products);
        }

        //GET: / api / products / 5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {

            var product = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "Category,Author,Cover");

            if (product == null)
                return NotFound();

            return Ok(_mapper.Map<Product, ProductDto>(product));
        }


        // PUT /api/products/1
        [HttpPut]
        public async Task<IActionResult> Upsert(int id, ProductDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var productInDb = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == id, includeProperties: "Category,Author,Cover");

            if (productInDb == null)
                return NotFound();

            var _mappedProduct = _mapper.Map(productDto, productInDb);

            await _product.Entity.UpdateAsync(_mappedProduct);
            await _product.CompleteAsync();

            return Ok();
        }


        //GET: / api / products/ id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            Product productInDb = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == id);

            if (productInDb == null)
                return NotFound();

            if (productInDb.ImageUrl != null)
            {
                string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productInDb.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
            }

            await _product.Entity.DeleteAsync(productInDb.Id);
            await _product.CompleteAsync();

            return Ok(productInDb);
        }
    }
}