using BankApp.Entity.Models;
using BankApp.Entity.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApp.BankingApi.Entity.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<CustomerApplication> CustomerApplications { get; set; }
        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        private string? GetCurrentUserId()
        {
            return _httpContextAccessor?.HttpContext?.User?.FindFirst("UserId")?.Value
                ?? _httpContextAccessor?.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        }

        public override int SaveChanges()
        {
            UpdateAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateAuditFields();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateAuditFields()
        {
            var currentUserId = GetCurrentUserId();

            var baseEntityEntries = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted);

            foreach (var entry in baseEntityEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = currentUserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = DateTime.Now;
                        entry.Entity.ModifiedBy = currentUserId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = DateTime.Now;
                        entry.Entity.DeletedBy = currentUserId;
                        break;
                }
            }

            var userEntries = ChangeTracker.Entries<ApplicationUser>()
                .Where(e => e.State == EntityState.Added ||
                           e.State == EntityState.Modified ||
                           e.State == EntityState.Deleted);

            foreach (var entry in userEntries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        entry.Entity.CreatedBy = currentUserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedDate = DateTime.Now;
                        entry.Entity.ModifiedBy = currentUserId;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedDate = DateTime.Now;
                        entry.Entity.DeletedBy = currentUserId;
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Customer>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
          //  builder.Entity<CustomerApplication>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Account>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<Transaction>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<ApplicationUser>().HasQueryFilter(e => !e.IsDeleted);

            builder.Entity<Customer>()
                .HasOne(c => c.ApplicationUser)
                .WithMany()
                .HasForeignKey(c => c.ApplicationUserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasOne(c => c.ApprovedByUser)
                .WithMany()
                .HasForeignKey(c => c.ApprovedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Employee>()
                .HasOne(e => e.ApplicationUser)
                .WithMany()
                .HasForeignKey(e => e.ApplicationUserID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Transaction>()
                .HasOne(t => t.RecipientAccount)
                .WithMany()
                .HasForeignKey(t => t.RecipientAccountID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Customer>()
                .HasIndex(c => c.MobileNumber)
                .IsUnique()
                .HasFilter("[IsDeleted]=0");
            builder.Entity<Customer>()
                .HasIndex(c => c.AadharNumber)
                .IsUnique()
                .HasFilter("[IsDeleted]=0");
            builder.Entity<Customer>()
                .HasIndex(c => c.PAN)
                .IsUnique()
                .HasFilter("[IsDeleted]=0");
            // ---- Indexes for CustomerApplications validation ----
            builder.Entity<CustomerApplication>()
                .HasIndex(a => a.MobileNumber);
            builder.Entity<CustomerApplication>()
                .HasIndex(a => a.AadharNumber);
            builder.Entity<CustomerApplication>()
                .HasIndex(a => a.PAN);

            var adminId = Guid.NewGuid().ToString();
            var adminUser = new ApplicationUser
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@bankapp.com",
                NormalizedEmail = "ADMIN@BANKAPP.COM",
                EmailConfirmed = true,
                FullName = "System Administrator",
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now,
                CreatedBy = "System"
            };

            var hasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

            builder.Entity<ApplicationUser>().HasData(adminUser);

            var adminRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });

            var managerRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = managerRoleId,
                Name = "Manager",
                NormalizedName = "MANAGER"
            });

            var customerRoleId = Guid.NewGuid().ToString();
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = customerRoleId,
                Name = "Customer",
                NormalizedName = "CUSTOMER"
            });

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminId
            });

            builder.Entity<AccountType>().HasData(
                new AccountType { AccountTypeID = 1, TypeName = "Savings", Description = "Savings Account", InterestRate = 0.0400m },
                new AccountType { AccountTypeID = 2, TypeName = "Current", Description = "Current Account", InterestRate = 0.0000m },
                new AccountType { AccountTypeID = 3, TypeName = "Fixed Deposit", Description = "Fixed Deposit Account", InterestRate = 0.0650m }
            );
        }
    }
}
