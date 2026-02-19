using IdentityService.Data.Configurations;
using IdentityService.Models;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    public override DbSet<IdentityUserRole<string>> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ApplicationUserEntityTypeConfiguration());
        builder.ApplyConfiguration(new PermissionEntityTypeConfiguration());
        builder.ApplyConfiguration(new RolePermissionEntityTypeConfiguration());

        base.OnModelCreating(builder);

        base.OnModelCreating(builder);

        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
