using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace eShopSolution.ViewModels.System.Users
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .NotEmpty().WithMessage("Password is required");
        }
    }
}