using TransportSchadules.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Route = TransportSchadules.Models.Route;

namespace TransportSchadules.Data
{
    public class TransportDbContext(DbContextOptions<TransportDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {

        public virtual DbSet<Personnel> Personnels { get; set; }

        public virtual DbSet<Route> Routes { get; set; }

        public virtual DbSet<Schedule> Schedules { get; set; }

        public virtual DbSet<Stop> Stops { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(e => e.Distance)
                    .HasColumnType("decimal(18, 4)"); // Укажите нужный тип и масштаб
            });
        }
    }
}
