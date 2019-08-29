using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models.ProductViewModels
{
    [NotMapped]
    public class ProductWithQuantity:Product
    {

        public int QuantitySold { get; set; }
    }
}
