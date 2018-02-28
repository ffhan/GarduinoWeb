using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Garduino.Hubs;
using Garduino.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.Entity<Device>().Property(b => b.Alive).HasField("_alive");

        }
        
        public DbSet<Garduino.Models.Entry> Measure { get; set; }

        public DbSet<Garduino.Models.Code> Code { get; set; }

        public DbSet<Garduino.Models.Device> Device { get; set; }

        public DbSet<User> User { get; set; }
    }
}
