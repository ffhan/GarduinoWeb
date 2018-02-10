using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Data
{
    public interface IBaseRepository<T>
    {
        Task<bool> AddAsync(T what, string userId);
        IEnumerable<T> GetAll(string userId);
        Task<T> GetAsync(Guid id, string userId);
        Task<T> GetAsync(T what, string userId);
        Task<bool> UpdateAsync(Guid id, T what, string userId);
        Task<bool> ContainsAsync(T what, string userId);
        Task<bool> ContainsAsync(Guid id, string userId);
        Task<bool> DeleteAsync(Guid id, string userId);
        Task<Guid> GetId(T what, string userId);
        Task<bool> DeleteAllAsync();
        Task AddAllAsync(ISet<T> all, string userId);
    }
}
