using Microsoft.EntityFrameworkCore;

namespace GarduinoWebAPI.Data
{
    public class MeasureContext : DbContext
    {
        public MeasureContext (DbContextOptions<MeasureContext> options)
            : base(options)
        {
        }

        public DbSet<GarduinoWebAPI.Models.Measure> Measure { get; set; }
    }
}
