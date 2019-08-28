using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bangazon.Models
{
    public class UserProductOpinion
    {

        public int Id { get; set; }

        public bool? Opinion { get; set; }

        public string UserId { get; set; }


        public int ProductId { get; set; }


    }
}
