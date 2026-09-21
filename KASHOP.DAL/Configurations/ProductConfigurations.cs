using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KASHOP.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KASHOP.DAL.Configurations
{
    public class ProductConfigurations: IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(x => x.Price).HasPrecision(6, 2);
            builder.Property(x => x.Discount).HasPrecision(6, 2);

            builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UpdatedBy).WithMany().HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
