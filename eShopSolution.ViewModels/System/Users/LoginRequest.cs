using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequest
    {
        [Required]
        public string UserName { set; get; }

        [Required]
        public string Password { get; set; }

        [DefaultValue(false)]
        public bool RememberMe { set; get; } = false;
    }
}