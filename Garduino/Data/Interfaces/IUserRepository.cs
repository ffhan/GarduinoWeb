using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetAsync(string id);
        Task<bool> AddAsync(User user);
    }
}
