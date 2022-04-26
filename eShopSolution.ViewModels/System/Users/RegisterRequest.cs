using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequest
    {
        public string FirstName { set; get; }

        public string LastName { set; get; }

        public DateTime? Dob { set; get; }

        public string email { set; get; }

        public string UserName { set; get; }

        public string Password { set; get; }

        public string ConfirmPassword { set; get; }
    }
}