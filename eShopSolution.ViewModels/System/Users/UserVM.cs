using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserVM
    {
        public Guid id { get; set; }
        public string UserName { get; set; }

        public DateTime Dob { get; set; }

        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}