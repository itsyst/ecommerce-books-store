using Books.Domain.Entities;
using Books.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Books.Controllers
{
#pragma warning disable

    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork<Product> _product;
        private readonly IUnitOfWork<ShoppingCart> _shoppingCart;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IUnitOfWork<Product> product,
            IUnitOfWork<ShoppingCart> shoppingCart,
            ILogger<HomeController> logger)
        {
            _product = product;
            _shoppingCart = shoppingCart;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var product = await _product.Entity.GetAllAsync(filter: null, c=>c.Category, a=>a.Author, r=>r.Cover);
            return View(product);
        }

        // GET: Home/Details/5
        public async Task<IActionResult> Details(int productId)
        {

            ShoppingCart cart = new()
            {
                Product = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == productId, includeProperties: "Category,Author,Cover"),
                ProductId = productId,
                Count = 0
            };

            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = userId;

            var shoppingCartInDb = await _shoppingCart.Entity.GetFirstOrDefaultAsync(c => c.ApplicationUserId == userId && c.ProductId == shoppingCart.ProductId);
            var product = await _product.Entity.GetFirstOrDefaultAsync(p => p.Id == shoppingCart.ProductId);


            if (shoppingCart.Count > product.InStock)
            {
                TempData["Error"] = "Not enough items In stock.";
                return RedirectToAction(nameof(Index));
            }

            if (shoppingCartInDb == null)
            {
                await _shoppingCart.Entity.InsertAsync(shoppingCart);
                await _shoppingCart.CompleteAsync();

                product.InStock -= shoppingCart.Count;
                await _product.Entity.UpdateAsync(product);
                await _product.CompleteAsync();
            }

            else
            {
                await _shoppingCart.Entity.UpdateAsync(shoppingCartInDb);

                shoppingCartInDb.Count += shoppingCart.Count;

                if (product.InStock > 0 && shoppingCart.Count <= product.InStock)
                {
                    product.InStock -= shoppingCart.Count;

                    await _product.Entity.UpdateAsync(product);
                    await _product.CompleteAsync();

                    await _shoppingCart.CompleteAsync();
                }
                else
                {
                    TempData["Error"] = "Not enough items In stock.";
                    return RedirectToAction(nameof(Index));
                }
                
            }
 
            return RedirectToAction(actionName:"Index",controllerName:"Cart");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}