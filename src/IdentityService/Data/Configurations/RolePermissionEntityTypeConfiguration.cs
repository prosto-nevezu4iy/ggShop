using IdentityService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Data.Configurations;

public class RolePermissionEntityTypeConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.Property(p => p.RoleId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });
    }
}