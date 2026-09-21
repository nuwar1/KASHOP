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
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {

            builder.HasOne(p => p.CreatedBy).WithMany().HasForeignKey(p => p.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UpdatedBy).WithMany().HasForeignKey(p => p.UpdatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
