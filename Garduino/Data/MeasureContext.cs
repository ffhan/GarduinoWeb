using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Garduino.Models
{
    public class MeasureContext : DbContext
    {
        public MeasureContext (DbContextOptions<MeasureContext> options)
            : base(options)
        {
        }

        public DbSet<Garduino.Models.Measure> Measure { get; set; }

        public override EntityEntry Add(object entity)
        {
            if(Measure.FirstOrDefault(g => g.Equals(entity)) == default(Measure)) return base.Add(entity);
            return null;
        }

        public override Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if(Measure.FirstOrDefault(g => g.Equals(entity)) == default(Measure)) return base.AddAsync(entity, cancellationToken);
            return null;
        }
    }
}
