using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IMeasureRepository : IBaseRepository<Measure>, IDeviceable<Measure>, ITimeable<Measure>
    {
        
        Task<Measure> GetAsync(DateTime dateTime, ApplicationUser user);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, ApplicationUser user);
        bool AreEqual(Measure m1, Measure m2);
        
    }
}
