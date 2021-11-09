using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.ToTable("ProductInCategories");
            builder.HasKey(t => new { t.CategoryId, t.ProductId });
            builder.HasOne(p => p.Product).WithMany(pr => pr.ProductInCategories)
                .HasForeignKey(pr => pr.ProductId);
            builder.HasOne(c => c.Category).WithMany(pc => pc.ProductInCategories)
                .HasForeignKey(pc => pc.CategoryId);
        }
    }
}
