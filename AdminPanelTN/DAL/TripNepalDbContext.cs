using AdminPanelTN.Model;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelTN.DAL
{
    public class TripNepalDbContext : DbContext
    {
        public TripNepalDbContext(DbContextOptions<TripNepalDbContext> options) : base(options)
        {
        }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<User> Users { get; set; }

    }
}
