using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IBaseRepository<T>
    {
        Task<bool> AddAsync(T what, ApplicationUser user);
        IEnumerable<T> GetAll(ApplicationUser user);
        Task<T> GetAsync(Guid id, ApplicationUser user);
        Task<T> GetAsync(T what, ApplicationUser user);
        Task<bool> UpdateAsync(Guid id, T what, ApplicationUser user);
        Task<bool> ContainsAsync(T what, ApplicationUser user);
        Task<bool> ContainsAsync(Guid id, ApplicationUser user);
        Task<bool> DeleteAsync(Guid id, ApplicationUser user);
        Task<Guid> GetId(T what, ApplicationUser user);
        Task<bool> DeleteAllAsync();
        Task AddAllAsync(ISet<T> all, ApplicationUser user);
    }
}
