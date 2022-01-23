using Books.Domain.Entities;
using Books.Domain.ViewModels;
using Books.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Books.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly IUnitOfWork<Product> _product;
        private readonly IUnitOfWork<Author> _author;
        private readonly IUnitOfWork<Category> _category;
        private readonly IUnitOfWork<Cover> _cover;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork<Product> product, IUnitOfWork<Category> category, IUnitOfWork<Cover> cover, IUnitOfWork<Author> author, IWebHostEnvironment hostEnvironment)
        {
            _product = product;
            _category = category;
            _cover = cover;
            _author = author;
            _hostEnvironment = hostEnvironment;
        }

        // GET: ProductController
        public async Task<ActionResult> Index()
        {
            IEnumerable<Product> products = await _product.Entity.GetAllAsync();
            return View(products);
        }

        // GET: ProductController/Upsert/5
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<Category> categories = await _category.Entity.GetAllAsync();
            IEnumerable<Cover> covers = await _cover.Entity.GetAllAsync();
            IEnumerable<Author> authors = await _author.Entity.GetAllAsync();

            ProductViewModel model = new()
            {
                Product = new(),

                Categories = categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Covers = covers.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                Authors = authors.Select(i => new SelectListItem
                {
                    Text = i.FullName,
                    Value = i.Id.ToString()
                })
            };

            if (id == null || id == 0)
            {
                // create product
                //ViewBag.Categories = model.Categories;
                //ViewData["Covers"] = model.Covers;
                return View(model);
            }
            else
            {
                // update product
                model.Product = await _product.Entity.GetFirstOrDefaultAsync(m => m.Id == id);
                return View(model);
            }
        }

        // POST: ProductController/Upsert/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductViewModel model, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName).ToLower();

                    if (model.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, model.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }


                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileStreams);
                    }

                    model.Product.ImageUrl = @"\images\products\" + fileName + extension;


                    ResizeImage(file, Path.Combine(uploads, fileName + extension));
                }

                if (model.Product.Id == 0)
                {
                    await _product.Entity.InsertAsync(model.Product);
                }
                else
                {
                    await _product.Entity.UpdateAsync(model.Product);
                }

                await _product.CompleteAsync();
                TempData["Success"] = "Product created successfully.";

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public void ResizeImage(IFormFile file, string path)
        {
            using var image = Image.Load(file.OpenReadStream());
            image.Mutate(x => x.Resize(456, 650));
            image.Save(path);
        }
    }
}
