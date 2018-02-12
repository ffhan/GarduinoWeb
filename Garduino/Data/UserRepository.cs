using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task<User> GetAsync(ApplicationUser user)
        {
            return await _context.User.FirstOrDefaultAsync(c => user.CleanUser.Equals(c));
        }

        public async Task AddAsync(User user)
        {
            if (user == null) throw new Exception("User is null.");
            await _context.User.AddAsync(user);
        }
    }
}
