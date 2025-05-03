using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjetERP.Areas.Identity.Data;
using ProjetERP.Models;

namespace ProjetERP.Data
{
    public class ProjetERPDbContext : IdentityDbContext<User>
    {
        public ProjetERPDbContext(DbContextOptions<ProjetERPDbContext> options)
            : base(options)
        {
        }

        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<SupplierPerformance> SupplierPerformances { get; set; }
        public DbSet<TermsAndConditions> TermsAndConditions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configuration des tables Identity
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

            // Configuration des tables pour la gestion des fournisseurs
            builder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Suppliers");
                entity.HasKey(e => e.SupplierId);
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasColumnType("varchar(100)").IsRequired();
                entity.Property(e => e.ContactEmail).HasColumnType("varchar(100)");
                entity.Property(e => e.Phone).HasColumnType("varchar(20)");
                entity.Property(e => e.Address).HasColumnType("text");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(); // Assuré non nullable
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime").IsRequired(); // Assuré non nullable
            });

            builder.Entity<Contract>(entity =>
            {
                entity.ToTable("Contracts");
                entity.HasKey(e => e.ContractId);
                entity.Property(e => e.ContractId).HasColumnName("ContractId").ValueGeneratedOnAdd();
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId").IsRequired();
                entity.Property(e => e.Title).HasColumnType("varchar(100)").IsRequired();
                entity.Property(e => e.StartDate).HasColumnType("date").IsRequired(); // Assuré non nullable
                entity.Property(e => e.EndDate).HasColumnType("date").IsRequired(); // Assuré non nullable
                entity.Property(e => e.Terms).HasColumnType("text");
                entity.Property(e => e.Status).HasColumnType("varchar(50)");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(); // Assuré non nullable
                entity.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId);
            });

            builder.Entity<SupplierPerformance>(entity =>
            {
                entity.ToTable("SupplierPerformances");
                entity.HasKey(e => e.PerformanceId);
                entity.Property(e => e.PerformanceId).HasColumnName("PerformanceId").ValueGeneratedOnAdd();
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId").IsRequired();
                entity.Property(e => e.EvaluationDate).HasColumnType("date").IsRequired(); // Assuré non nullable
                entity.Property(e => e.Score).HasColumnType("int");
                entity.Property(e => e.Comments).HasColumnType("text");
                entity.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId);
            });

            builder.Entity<TermsAndConditions>(entity =>
            {
                entity.ToTable("TermsAndConditions");
                entity.HasKey(e => e.TermsId);
                entity.Property(e => e.TermsId).HasColumnName("TermsId").ValueGeneratedOnAdd();
                entity.Property(e => e.SupplierId).HasColumnName("SupplierId").IsRequired();
                entity.Property(e => e.Content).HasColumnType("text").IsRequired();
                entity.Property(e => e.Version).HasColumnType("int");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").IsRequired(); // Assuré non nullable
                entity.HasOne(e => e.Supplier).WithMany().HasForeignKey(e => e.SupplierId);
            });
        }
    }
}