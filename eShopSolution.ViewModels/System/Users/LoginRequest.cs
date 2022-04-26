using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequest
    {
        public string UserName { set; get; }

        public string Password { get; set; }

        [DefaultValue(false)]
        public bool RememberMe { set; get; } = false;
    }
}