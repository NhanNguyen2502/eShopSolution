using eShopSolution.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace eShopSolution.Data.Configurations
{
    class ProductTranslationConfiguration : IEntityTypeConfiguration<ProductTranslation>
    {
        public void Configure(EntityTypeBuilder<ProductTranslation> builder)
        {
            builder.ToTable("ProductTranlations");
            builder.HasKey(x => new { x.ProductId, x.LanguageId });
            builder.HasOne(p => p.Product).WithMany(pt => pt.productTranslations).HasForeignKey(pt => pt.ProductId);
            builder.HasOne(l => l.Language).WithMany(pt => pt.ProductTranslations).HasForeignKey(pt => pt.LanguageId);
        }
    }
}
