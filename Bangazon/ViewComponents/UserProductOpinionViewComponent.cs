using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.ViewComponents
{
    public class UserProductOpinion
    {

        public int Id { get; set; }

        public bool? Opinion { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }

        public ApplicationUser User { get; set; }

        public Product Product { get; set; }
    }

    public class UserProductOpinionViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        private Task<ApplicationUser> GetUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        public UserProductOpinionViewComponent(ApplicationDbContext c, UserManager<ApplicationUser> userManager)
        {
            _context = c;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(int ProductId)
        {
            // Get the current, authenticated user
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            // Instantiate view model
            UserProductOpinion model = new UserProductOpinion();

            // Determine return like and disliked products
            var opinion = await _context.UserProductOpinions
                .SingleOrDefaultAsync()
                ;

            // If there is an open order, query appropriate values

            // Render template bound to OrderCountViewModel
            return View(model);
        }
    }
}
