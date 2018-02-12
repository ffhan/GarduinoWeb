using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IBaseRepository<T>
    {
        Task<bool> AddAsync(T what, User user);
        IEnumerable<T> GetAll(User user);
        Task<T> GetAsync(Guid id, User user);
        Task<T> GetAsync(T what, User user);
        Task<bool> UpdateAsync(Guid id, T what, User user);
        Task<bool> ContainsAsync(T what, User user);
        Task<bool> ContainsAsync(Guid id, User user);
        Task<bool> DeleteAsync(Guid id, User user);
        Task<Guid> GetId(T what, User user);
        Task<bool> DeleteAllAsync();
        Task AddAllAsync(ISet<T> all, User user);
    }
}
