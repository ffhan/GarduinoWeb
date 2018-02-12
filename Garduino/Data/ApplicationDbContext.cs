using Garduino.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            /*
            builder.Entity<UserProfile>()
                .HasMany(up => up.Courses)
                .WithMany(course => course.UserProfiles)
                .Map(mc =>
                {
                    mc.ToTable("T_UserProfile_Course");
                    mc.MapLeftKey("UserProfileID");
                    mc.MapRightKey("CourseID");
                }
            );
            */

            builder.Entity<ApplicationUser>()
                .HasMany(c => c.Devices)
                .WithOne(c => c.User);
            builder.Entity<Device>()
                .HasMany(c => c.Codes)
                .WithOne(c => c.Device);
            builder.Entity<Device>()
                .HasMany(c => c.Measures)
                .WithOne(c => c.Device);


            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Garduino.Models.Measure> Measure { get; set; }

        public DbSet<Garduino.Models.Code> Code { get; set; }

        public DbSet<Garduino.Models.Device> Device { get; set; }
    }
}
