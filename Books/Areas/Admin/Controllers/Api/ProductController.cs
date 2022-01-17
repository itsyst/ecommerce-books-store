using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Books.Areas.Admin.Controllers.Api
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork<Product> _product;

        public ProductController(IUnitOfWork<Product> product)
        {
            _product = product;
        }

        [HttpGet(Name = "GetProducts")]
        [HttpHead]
        // GET: / api / product
        public async Task<IActionResult> GetProducts()
        {
            var products = await _product.Entity.GetAllAsync();
            //return Ok(products);
            return Ok(products);
        }
    }
}
