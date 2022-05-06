using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                .WithMessage("Email format dose not match.");
            RuleFor(x => x.PhoneNumber)
                .Cascade(CascadeMode.Stop)
                .Matches(@"[0-9]{10}")
                .WithMessage("PhoneNumber format does not match.");
        }
    }
}