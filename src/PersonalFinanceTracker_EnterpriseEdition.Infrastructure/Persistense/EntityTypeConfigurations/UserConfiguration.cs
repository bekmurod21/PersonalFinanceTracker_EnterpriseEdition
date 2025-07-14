using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Entities;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Enums;

namespace PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Persistense.EntityTypeConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.IsDeleted);
        builder.HasQueryFilter(u => !u.IsDeleted);
        builder.HasData(new User()
        {
            Id = Guid.NewGuid(), Email = "boqiyev482@gmail.com", Password = "Shunchaki21".Hash(),
            Role = ERole.Admin
        });
    }
}