using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Bangazon.Data;
using Bangazon.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bangazon.Areas.Identity.Pages.Account.Manage
{
    public class PaymentTypeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;


        public PaymentTypeModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public ICollection<PaymentType> PaymentTypes { get; set; }
        public int? PaymentTypeCount { get { return PaymentTypes.Count(); } }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [DataType(DataType.Date)]
            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public DateTime DateCreated { get; set; }

            [Required]
            [StringLength(55)]
            public string Description { get; set; }

            [Required]
            [StringLength(20)]
            public string AccountNumber { get; set; }
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            PaymentTypes = await _context.PaymentType
                .Where(pt => pt.UserId == user.Id).ToListAsync();

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var paymentType = new PaymentType()
            {
                UserId = user.Id,
                Description = Input.Description,
                AccountNumber = Input.AccountNumber,
                User = user
            };

            _context.Add(paymentType);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var paymentType = await _context.PaymentType
                .Where(pt => pt.PaymentTypeId == id).SingleAsync();

            _context.PaymentType.Remove(paymentType);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}