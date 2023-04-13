using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBankAssignment.Models;
using MyBankAssignment.ViewModels;

namespace MyBankAssignment.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<ClientAccount> ClientAccounts { get; set; }
        public DbSet<BankAccountType> BankAccountTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BankAccountType>()
                .Property(ba =>  ba.AccountType )
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<ClientAccount>()
                .HasKey(ca => new { ca.ClientID, ca.AccountNum });

            modelBuilder.Entity<ClientAccount>()
                .HasOne(p => p.Client)
                .WithMany(p => p.ClientAccounts)
                .HasForeignKey(fk => new { fk.ClientID })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClientAccount>()
                .HasOne(p => p.BankAccount)
                .WithMany(p => p.ClientAccounts)
                .HasForeignKey(fk => new { fk.AccountNum })
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BankAccount>()
                .Property(p => p.Balance)
                .HasColumnType("decimal(9,2)");

            modelBuilder.Entity<BankAccount>()
                .HasOne<BankAccountType>(b => b.BankAccountTypes)
                .WithMany(ba => ba.BankAccounts)
                .HasForeignKey(a => a.AccountType);

            modelBuilder.Entity<Client>()
                .Property(p => p.FirstName)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<Client>()
                .Property(p => p.LastName)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<Client>()
                .Property(p => p.Email)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<BankAccountType>()
            .HasData(new BankAccountType { AccountType = "Chequing" }
                    , new BankAccountType { AccountType = "Savings" }
                    , new BankAccountType { AccountType = "Investment" }
                    , new BankAccountType { AccountType = "RRSP" }
                    , new BankAccountType { AccountType = "RESP" }
                    , new BankAccountType { AccountType = "Tax Free Savings" }
            );
        }
        public DbSet<MyBankAssignment.ViewModels.ClientAccountVM> ClientAccountVM { get; set; }
        public DbSet<MyBankAssignment.ViewModels.BankAccountVM> BankAccountVM { get; set; }
    }
}