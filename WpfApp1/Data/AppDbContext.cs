using AutoDealerExam.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoDealerExam.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Automobil> Automobile => Set<Automobil>();
        public DbSet<Client> Clienti => Set<Client>();
        public DbSet<Comanda> Comenzi => Set<Comanda>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=autodealer.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Comanda>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Comenzi)
                .HasForeignKey(c => c.IdClient)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comanda>()
                .HasOne(c => c.Automobil)
                .WithMany(a => a.Comenzi)
                .HasForeignKey(c => c.IdAutomobil)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}