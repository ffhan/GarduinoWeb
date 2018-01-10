using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IEntryRepository
    {
        Task<bool> AddAsync(Measure measure, string userId);
        IEnumerable<Measure> GetAll(string userId);
        Task<Measure> GetAsync(Guid id, string userId);
        Task<Measure> GetAsync(DateTime dateTime, string userId);
        Task<Measure> GetAsync(Measure measure, string userId);
        Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId);
        Task<bool> UpdateAsync(Guid id, Measure measure, string userId);
        Task<bool> ContainsAsync(Measure measure, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
        Task<Guid> GetId(Measure measure, string userId);
        bool AreEqual(Measure m1, Measure m2);
        Task<bool> DeleteAllAsync();
    }
}
