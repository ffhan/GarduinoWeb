using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IMeasureRepository : IBaseRepository<Measure>, IDeviceable<Measure>, ITimeable<Measure>
    {
        
        Task<Measure> GetAsync(DateTime dateTime, User user);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, User user);
        bool AreEqual(Measure m1, Measure m2);
        
    }
}
