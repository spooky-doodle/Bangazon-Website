using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductTypeViewModel
{
    public class ProductTypeDetailViewModel
    {
        public int ProductTypeId { get; set; }
        public string Label { get; set; }
        public int ProductCount { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
