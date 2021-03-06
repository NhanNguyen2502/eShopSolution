using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public DateTime Dob { set; get; }
        public DateTime CreatedAt { set; get; }

        public List<Cart> Carts { set; get; }
        public List<Transaction> Transactions { set; get; }
        public List<Order> Orders { set; get; }
    }
}