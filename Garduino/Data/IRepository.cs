using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IRepository<T>
    {
        Task<bool> AddAsync(T what, string userId);
        IEnumerable<T> GetAll(string userId);
        IEnumerable<T> GetDevice(string device, string userId);
        Task<T> GetAsync(Guid id, string userId);
        Task<T> GetAsync(DateTime dateTime, string userId);
        Task<T> GetAsync(T what, string userId);
        Task<IEnumerable<T>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId);
        Task<bool> UpdateAsync(Guid id, T what, string userId);
        Task<bool> ContainsAsync(T what, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
        Task<Guid> GetId(T what, string userId);
        bool AreEqual(T m1, T m2);
        Task<bool> DeleteAllAsync();
        T GetLatest(string userId);
        Task AddAllAsync(ISet<T> all, string userId);
    }
}
