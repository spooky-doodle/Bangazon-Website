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
using Grpc.Core;
using System.Web;
using Microsoft.AspNetCore.Http;
using Bangazon.Models.ProductTypeViewModel;

namespace Bangazon.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
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

            return View(product);
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

                //string path = Path.Combine(Server.MapPath("~/images"), Path.GetFileName(file.FileName));
                //https://docs.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-2.2
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // GET: Products/Edit/5
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
            ViewData["ProductTypeId"] = new SelectList(_context.ProductType, "ProductTypeId", "Label", product.ProductTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", product.UserId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }


    }
}
