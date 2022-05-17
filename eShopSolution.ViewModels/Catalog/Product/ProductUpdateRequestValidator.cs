using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.ViewModels.Catalog.Product
{
    internal class ProductUpdateRequestValidator : AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator()
        {
            this.CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            RuleFor(x => x.NeworOriginalPrice)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            RuleFor(x => x.NewPrice)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            RuleFor(x => x.SeoTitle)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
            RuleFor(x => x.Stock)
                .Cascade(CascadeMode.Stop)
                .NotEmpty();
        }
    }
}