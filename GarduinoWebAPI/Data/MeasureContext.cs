using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GarduinoWebAPI.Models
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
