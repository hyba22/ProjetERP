using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetERP.Areas.Identity.Data;

namespace ProjetERP.Data
{
    public class ProjetERPDbContext : IdentityDbContext<User>
    {
        public ProjetERPDbContext(DbContextOptions<ProjetERPDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.Property(e => e.Id).HasColumnType("varchar(255)");
                entity.Property(e => e.FirstName).HasColumnType("varchar(100)");
                entity.Property(e => e.LastName).HasColumnType("varchar(100)");
                entity.Property(e => e.Role).HasConversion<string>().HasColumnType("varchar(50)");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("Roles");
                entity.Property(e => e.Id).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles");
                entity.Property(e => e.UserId).HasColumnType("varchar(255)");
                entity.Property(e => e.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims");
                entity.Property(e => e.UserId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins");
                entity.Property(e => e.UserId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims");
                entity.Property(e => e.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens");
                entity.Property(e => e.UserId).HasColumnType("varchar(255)");
            });
        }
    }
}