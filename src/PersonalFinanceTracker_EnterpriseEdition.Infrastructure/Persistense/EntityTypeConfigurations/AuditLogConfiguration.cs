using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense.EntityTypeConfigurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId);
    }
} 