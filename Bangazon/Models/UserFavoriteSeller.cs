using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models
{
    public class UserFavoriteSeller
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string SellerId { get; set; }

        public bool Favorite { get; set; }
    }
}
