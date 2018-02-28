using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface IMeasureRepository : IBaseRepository<Entry, Device>, IDeviceable<Entry>, ITimeable<Entry, Device>
    {
        
        Task<Entry> GetAsync(DateTime dateTime, Device device);
        Task<IEnumerable<Entry>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device);
        bool AreEqual(Entry m1, Entry m2);
        
    }
}
