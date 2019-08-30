using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace Bangazon.Models.OrderViewModels
{
    public class OrderDetailViewModel
    {
        public Order Order { get; set; }

        public IEnumerable<OrderLineItem> LineItems { get; set; }
        public double Total
        {
            get
            {
                return LineItems.Select(i => i.Cost).Sum();
            }
        }

        public List<SelectListItem> PaymentOptions { get; set; }

        public int PaymentTypeId { get; set; }
    }

}