//st10440432
//Matteo Nusca
using Microsoft.EntityFrameworkCore;
using BookingSystemCLVD.Models;

namespace BookingSystemCLVD.Data // Define the data namespace.
{
    public class ApplicationDbContext : DbContext // take from DbContext.
    {
        // Constructor to configure DbContext.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Set for the Venue entity.
        public DbSet<Venue> Venues { get; set; }

        // Set for the Event entity.
        public DbSet<Event> Events { get; set; }

        // Set for the Booking entity.
        public DbSet<Booking> Bookings { get; set; }
    }
}