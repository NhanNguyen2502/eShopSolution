using NPOI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserUpdateRequest
    {
        public Guid id { get; set; }
        [Display(Name = "Họ")]
        public string FirstName { set; get; }

        [Display(Name = "Tên")]
        public string LastName { set; get; }

        [Display(Name = "Ngày Sinh")]
        public DateTime? Dob { set; get; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }
    }
}