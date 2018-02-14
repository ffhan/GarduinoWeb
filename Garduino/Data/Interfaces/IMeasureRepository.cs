using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface IMeasureRepository : IBaseRepository<Measure, Device>, IDeviceable<Measure>, ITimeable<Measure, Device>
    {
        
        Task<Measure> GetAsync(DateTime dateTime, Device device);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device);
        bool AreEqual(Measure m1, Measure m2);
        
    }
}
