using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense.EntityTypeConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasIndex(c => c.IsDeleted);
        builder.HasQueryFilter(c => !c.IsDeleted);
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
} 