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
using Bangazon.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;

namespace Bangazon.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Cart
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                return View(await CreateCartViewModel(user));
            }
            else
            {
                return NotFound();
            }
        }





        // get products from list of OProducts.
        private ICollection<OrderLineItem> GetLineItems(ICollection<OrderProduct> orderProducts)
        {
            // second param of this groupBy is: key(int) is productId/ group is list of orderproducts that correspond to the shared key
            return orderProducts.GroupBy(p => p.ProductId, (key, group) =>
           {
               return new OrderLineItem()
               {
                   Units = group.Count(),
                   Product = group.First().Product
               };
           }).ToList();

        }

        //Delete From Cart
        [HttpPost, ActionName("RemoveFromCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart([FromRoute] int id)
        {
            var foundOrder = await GetOrder();
            var orderProduct = await _context.OrderProduct.FirstOrDefaultAsync(op => op.ProductId == id && op.OrderId == foundOrder.OrderId);
            _context.OrderProduct.Remove(orderProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction("Cart", "Orders");
        }





        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var applicationDbContext = _context.Order
                                            .Include(o => o.PaymentType)
                                            .Include(o => o.User)
                                            .Where(o => user.Id == o.UserId && o.DateCompleted == null);
                return View(await applicationDbContext.ToListAsync());
            }
            else
            {
                return NotFound();
            }
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.PaymentType)
                .Include(o => o.User)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,DateCreated,DateCompleted,UserId,PaymentTypeId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,DateCreated,DateCompleted,UserId,PaymentTypeId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            ViewData["PaymentTypeId"] = new SelectList(_context.PaymentType, "PaymentTypeId", "AccountNumber", order.PaymentTypeId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", order.UserId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return NotFound();

            var viewModel = await CreateCartViewModel(user);


            if (user == null || viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int? id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (id != null)
            {
                var order = await GetOrderWithProducts((int)id);
                if (order == null || order.UserId != user.Id) return NotFound();

                _context.OrderProduct.RemoveRange(order.OrderProducts);
                _context.Order.Remove(order);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index), "Home");
            }
            else return NotFound();
        }


        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            OrderDetailViewModel viewModel = null;
            if (user != null)
            {
                viewModel = await CreateCartViewModel(user);
                viewModel.PaymentOptions = (await GetUserPaymentTypes(user))
                    .Select(pt => new SelectListItem()
                    {
                        Text = pt.Description,
                        Value = pt.PaymentTypeId.ToString()
                    }).ToList();


                return View(viewModel);
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteCheckout(int PaymentTypeId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var order = await GetOrder();
            if (order == null) return NotFound();
            var dateCompleted = DateTime.UtcNow;
            order.DateCompleted = dateCompleted;
            order.PaymentTypeId = PaymentTypeId;
            await _context.SaveChangesAsync();



            return RedirectToAction(nameof(ThankYou), new { orderId = order.OrderId});
        }

        [Authorize]
        public async Task<IActionResult> ThankYou(int orderId)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var order = await _context.Order.FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == user.Id);
            if (order != null)
            {
                var epoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


                var viewModel = new ThankYouViewModel()
                {
                    ConfirmationNumber = $"{epoch}-{user.Id}"
                };
                return View(viewModel);
            }
            else return NotFound();
            
        }

        //    ************************
        //    ***  HELPER METHODS  ***
        //    ************************ 



        // helper method to get or create order for user
        public async Task<Order> GetOrder()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return await _context.Order
                                .Where(o => o.DateCompleted == null)
                                .FirstOrDefaultAsync(o => o.UserId == user.Id);
        }

        public async Task<Order> GetOrderWithProducts(int id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return await _context.Order
                                .Where(o => o.DateCompleted == null && o.OrderId == id)
                                .Include(o => o.OrderProducts)
                                .FirstOrDefaultAsync(o => o.UserId == user.Id);
        }

        public async Task<ICollection<PaymentType>> GetUserPaymentTypes(ApplicationUser user)
        {
            return await _context.PaymentType.Where(pt => pt.UserId == user.Id).ToListAsync();
        }


        private async Task<OrderDetailViewModel> CreateCartViewModel(ApplicationUser user)
        {
            var viewModel = new OrderDetailViewModel();
            viewModel.Order = await _context.Order
                                            .Include(o => o.User)
                                            .Include(o => o.OrderProducts)
                                            .ThenInclude(Op => Op.Product)
                                            .Where(o => user.Id == o.UserId && o.DateCompleted == null)
                                            .FirstOrDefaultAsync();
            if (viewModel.Order == null) { viewModel = null; }
            else
            {
                viewModel.LineItems = GetLineItems(viewModel.Order.OrderProducts);
            }
            return viewModel;
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
