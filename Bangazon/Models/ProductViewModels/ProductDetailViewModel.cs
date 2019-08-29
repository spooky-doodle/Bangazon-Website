using Bangazon.Models;
using Bangazon.Data;

namespace Bangazon.Models.ProductViewModels
{
  public class ProductDetailViewModel
  {
    public ApplicationUser User { get; set; }
    public Product Product { get; set; }
  }
}