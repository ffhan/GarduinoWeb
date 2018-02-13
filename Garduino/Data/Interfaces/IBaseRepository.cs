﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garduino.Data.Interfaces
{
    public interface IBaseRepository<T, U>
    {
        
        IEnumerable<T> GetAll(U userId);
        Task<T> GetAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, T what);
        Task<bool> ContainsAsync(T what, U userId);
        Task<bool> ContainsAsync(Guid id, U userId);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> DeleteAllAsync();
        Task AddAllAsync(ISet<T> all, U userId);
    }
}
