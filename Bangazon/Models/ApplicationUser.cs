using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Bangazon.ViewComponents;
using Microsoft.AspNetCore.Identity;

namespace Bangazon.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
             
        }

        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public virtual ICollection<Product> Products { get; set; }
        [NotMapped]
        public virtual ICollection<Order> Orders { get; set; }
        [NotMapped]
        public virtual ICollection<PaymentType> PaymentTypes { get; set; }
        [NotMapped]
        public virtual ICollection<UserProductOpinion> ProductsOpinion { get; set; }


    }
}
