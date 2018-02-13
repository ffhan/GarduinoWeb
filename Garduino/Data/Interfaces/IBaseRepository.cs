using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garduino.Data.Interfaces
{
    public interface IBaseRepository<T, U>
    {
        
        IEnumerable<T> GetAll(U userId);
        Task<T> GetAsync(Guid id, U userId);
        Task<T> GetAsync(T what, U userId);
        Task<bool> UpdateAsync(Guid id, T what, U userId);
        Task<bool> ContainsAsync(T what, U userId);
        Task<bool> ContainsAsync(Guid id, U userId);
        Task<bool> DeleteAsync(Guid id, U userId);
        Task<Guid> GetId(T what, U userId);
        Task<bool> DeleteAllAsync();
        Task AddAllAsync(ISet<T> all, U userId);
    }
}
