using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Bangazon.Models.ProductViewModels;
using System.IO;
using System.Web;
using Microsoft.AspNetCore.Http;
using Bangazon.Models.ProductTypeViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IHostingEnvironment _env;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IHostingEnvironment env
            )
        {
            _env = env;
            _context = context;
            _userManager = userManager;
        }

        private readonly UserManager<ApplicationUser> _userManager;

        // GET: Products
        public async Task<IActionResult> Index(string userInput, int? pageNumber)
        {
            var userInputNotEmpty = !String.IsNullOrEmpty(userInput);
            if (userInputNotEmpty)
            {
                pageNumber = 1;
                var applicationDbContext = _context.Product
                                            .Include(p => p.ProductType)
                                            .Include(p => p.User)
                                            .Where(p => p.Title.Contains(userInput) || p.Description.Contains(userInput) || p.City.Contains(userInput))
                                            .OrderByDescending(p => p.DateCreated);
                int pageSize = 20;
                return View(await PaginatedList<Product>.CreateAsync(applicationDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));

            }
            else
            {

                var applicationDbContext = _context.Product
                                            .Include(p => p.ProductType)
                                            .Include(p => p.User)
                                            .OrderByDescending(p => p.DateCreated);
                int pageSize = 20;
                return View(await PaginatedList<Product>.CreateAsync(applicationDbContext.AsNoTracking(), pageNumber ?? 1, pageSize));
            }


        }

        // GET: Products/MyProducts
        public async Task<IActionResult> MyProducts(string userInput)
        {
            var userInputNotEmpty = !String.IsNullOrEmpty(userInput);
            var user = await GetUserAsync();
            if (userInputNotEmpty)
            {
                var applicationDbContext = _context.Product
                                            .Include(p => p.ProductType)
                                            .Include(p => p.User)
                                            .Where(p => p.Title.Contains(userInput) && p.UserId == user.Id);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                var applicationDbContext = _context.Product.Include(p => p.ProductType).Include(p => p.User).OrderByDescending(p => p.DateCreated).Where(p => p.UserId == user.Id); ;
                return View(await applicationDbContext.ToListAsync());
            }


        }



        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await GetUserAsync();
            
            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var viewModel = new ProductDetailViewModel()
            {
                Product = product,
                User = user
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Types()
        {

            // Build list of Product instances for display in view
            // LINQ is awesome
            var prods = await (
                from t in _context.ProductType
                join p in _context.Product
                on t.ProductTypeId equals p.ProductTypeId
                group new { t, p } by new { t.ProductTypeId, t.Label } into grouped
                select new GroupedProducts
                {
                    TypeId = grouped.Key.ProductTypeId,
                    TypeName = grouped.Key.Label,
                    ProductCount = grouped.Select(x => x.p.ProductId).Count(),
                    Products = grouped.Select(x => x.p).Take(3)
                }).ToListAsync();

            return View(prods.AsEnumerable());
        }

        public async Task<IActionResult> ProductTypeList(int id)
        {
            var viewModel = new ProductTypeListViewModel();

            viewModel.ProductTypeId = id;

            var productType = await _context.ProductType
                .Include(pt => pt.Products)
                .Where(pt => pt.ProductTypeId == id).SingleAsync();

            viewModel.ProductType = productType;
            viewModel.Label = productType.Label;
            viewModel.Products = productType.Products;

            viewModel.ProductCount = viewModel.Products.Count();

            return View(viewModel);
        }


        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProductId,Description,Title,Price,Quantity,City,Active,ProductTypeId")] Product product,
            IFormFile file)
        {
            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                var user = await GetUserAsync();
                product.UserId = user.Id;
                try
                {
                    product.ImagePath = await SaveFile(file, user.Id);
                } catch (Exception ex)
                {
                    return NotFound();
                }


                //string path = Path.Combine(Server.MapPath("~/images"), Path.GetFileName(file.FileName));
                //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.2
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = product.ProductId});
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user.Id == product.UserId)
            {
                ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
                ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
                return View(product);
            }

            return NotFound();
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,DateCreated,Description,Title,Price,Quantity,UserId,City,ImagePath,Active,ProductTypeId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .Include(p => p.ProductType)
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Add Product to Order
        [Authorize]
        [HttpPost, ActionName("ProductToCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductToCart(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var product = await _context.Product.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            if (user != null)
            {
                var order = await GetOrder(user);
                if(order == null)
                {
                    order = await CreateOrder(user);
                }

                if (_context.OrderProduct.Any(op => op.ProductId == id))
                {
                    var foundOrder = _context.Order
                        .Include(o => o.OrderProducts)
                        .Where(o => o.UserId == user.Id);
                }
                _context.OrderProduct.Add(new OrderProduct()
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId
                });
                await _context.SaveChangesAsync();
                return RedirectToAction("Cart", "Orders");
            }
            return RedirectToAction("LogIn", "Account", new { area = "" });
        }

        private object GetLineItems(OrderProduct foundOrderProduct)
        {
            throw new NotImplementedException();
        }

        // helper method to get or create order for user
        public async Task<Order> GetOrder(ApplicationUser user)
        {
            return await _context.Order
                                .Where(o => o.DateCompleted == null)
                                .FirstOrDefaultAsync(o => o.UserId == user.Id);

        }

        public async Task<Order> CreateOrder(ApplicationUser user)
        {
            var newOrder = new Order()
            {
                UserId = user.Id
            };
            _context.Order.Add(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private async Task<string> SaveFile(IFormFile file, string userId)
        {
            if (file.Length > 5242880) throw new Exception("File too large!");
            var ext = GetMimeType(file.FileName);
            if (ext == null) throw new Exception("Invalid file type");

            var epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            var fileName = $"{epoch}-{userId}.{ext}";
            var webRoot = _env.WebRootPath;
            var absoluteFilePath = Path.Combine(
                webRoot,
                "images",
                fileName);
            string relFilePath = null;
            if (file.Length > 0)
            {
                using (var stream = new FileStream(absoluteFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                    relFilePath = $"~/images/{fileName}";
                };
            }


            return relFilePath;
        }


        private string GetMimeType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            provider.TryGetContentType(fileName, out contentType);
            if (contentType == "image/jpeg") contentType = "jpg";
            else contentType = null;

            return contentType;
        }
    }
}
