using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IMeasureRepository : IBaseRepository<Measure, string>, IDeviceable<Measure>, ITimeable<Measure>
    {
        
        Task<Measure> GetAsync(DateTime dateTime, string userId);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId);
        bool AreEqual(Measure m1, Measure m2);
        
    }
}
