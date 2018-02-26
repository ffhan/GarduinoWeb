using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Data.Interfaces;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context) => _context = context;
        public async Task<User> GetAsync(string id)
        {
            return await _context.User.Include(c => c.Devices).FirstOrDefaultAsync(g => StringOperations.IsFromUser(g.Id, id));
        }

        public async Task<bool> AddAsync(User user)
        {
            try
            {
                await _context.User.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            if (user.Id == null) return false;
            User usr = await GetAsync(user.Id);
            if (usr == null) return false;
            usr.Update(user);
            _context.Entry(usr).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }
    }
}
