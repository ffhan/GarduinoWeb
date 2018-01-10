using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GarduinoAPI.Models;

namespace GarduinoAPI.Data
{
    public interface IEntryRepository
    {
        Task<bool> AddAsync(Measure measure);
        IEnumerable<Measure> GetAll();
        Task<Measure> GetAsync(Guid id);
        Task<Measure> GetAsync(DateTime dateTime);
        Task<Measure> GetAsync(Measure measure);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2);
        Task<bool> UpdateAsync(Guid id, Measure measure);
        Task<bool> ContainsAsync(Measure measure);
        Task<bool> DeleteAsync(Guid id);
        Task<Guid> GetId(Measure measure);
        bool AreEqual(Measure m1, Measure m2);
        Task<bool> DeleteAllAsync();
    }
}
