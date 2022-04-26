using eShopSolution.Data.EF;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eShopSolution.ViewModels.System.Users
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("First Name is Required.")
                .MaximumLength(25).WithMessage("First Name can not over 25 characters.");
            RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Last Name is Required.")
                .MaximumLength(25).WithMessage("First Name can not over 25 characters.");
            RuleFor(x => x.Dob)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .GreaterThan(DateTime.Now.AddYears(-100))
                .WithMessage("Birthday can not greater than 100 years");
            RuleFor(x => x.email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is Required.")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")
                .WithMessage("Email format not match.");
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("User Name is Required.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .Matches("(?=.*[A-Z])(?=.*[@$!%*#?&]){6,}")
                .WithMessage("Password can not less 6 characters.,There is at least a uppercase,There is at least a special character");
            RuleFor(x => x.ConfirmPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("ConfirmPassword is Required.");
            RuleFor(x => x).Cascade(CascadeMode.Stop).Custom((requset, context) =>
            {
                if (requset.ConfirmPassword != requset.Password)
                {
                    context.AddFailure("Confirm password is not match password.");
                }
            });
        }
    }
}