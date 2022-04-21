using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { set; get; }

        [Required]
        public string LastName { set; get; }

        public DateTime Dob { set; get; }

        [Required]
        public string email { set; get; }

        [Required]
        public string UserName { set; get; }

        [Required]
        public string Password { set; get; }

        [Required]
        public string ConfirmPassword { set; get; }
    }
}