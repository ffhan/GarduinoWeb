using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class CodeRepository : ICodeRepository //TODO: schedule codes
    { //TODO: Register code for reuse, then use them like this.
        private readonly ApplicationDbContext _context;

        public CodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Code code, Device device)
        {
            code.DateArrived = DateTime.Now;
            try
            {
                await _context.Code.AddAsync(code);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Code> GetAll(string userId)
        {
            return _context.Code.Where(g => g.Device.ApplicationUser.Id.Equals(userId)).OrderByDescending(g => g);
        }

        public Code GetLatest(string userId)
        {
            return GetAll(userId)?.FirstOrDefault(g => !g.IsCompleted);
        }

        public IEnumerable<Code> GetActive(string userId) => _context.Code.Where(g => g.Device.ApplicationUser.Id.Equals(userId) && !g.IsCompleted).OrderByDescending(g => g);

        public async Task Complete(Code code, DateTime dateExecuted, string userId)
        {
            code.Complete(dateExecuted);
            await UpdateAsync(code.Id, code, userId);
        }

        public async Task Complete(Guid id, DateTime dateExecuted, string userId)
        {
            Code code = await GetAsync(id, userId);
            code.Complete(dateExecuted);
            await UpdateAsync(id, code, userId);
        }

        public IEnumerable<Code> GetDeviceFromActiveCodes(string device, string userId)
        {
            return _context.Code.Where(g => g.Device.ApplicationUser.Id.Equals(userId) && g.IsFromDevice(device) && !g.IsCompleted);
        }

        public IEnumerable<Code> GetDevice(string device, string userId)
        {
            return _context.Code.Where(g => g.Device.ApplicationUser.Id.Equals(userId) && g.IsFromDevice(device));
        }

        public async Task<Code> GetAsync(Guid id, string userId)
        {
            return await _context.Code.FirstOrDefaultAsync(g => g.Id.Equals(id) && g.Device.ApplicationUser.Id.Equals(userId));
        }

        public async Task<Code> GetAsync(DateTime dateTime, string userId)
        {
            return await _context.Code.FirstOrDefaultAsync(g =>
                g.DateArrived.Equals(dateTime) && g.Device.ApplicationUser.Id.Equals(userId));
        }

        public async Task<Code> GetAsync(Code what, string userId)
        {//TODO: SWITCH TO STRINGOPERATIONS
            return await _context.Code.FirstOrDefaultAsync(g => g.Equals(what) && g.Device.ApplicationUser.Id.Equals(userId));
        }

        public async Task<IEnumerable<Code>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId)
        {
            return _context.Code.Where(g => g.DateArrived.CompareTo(dateTime1) >= 0 &&
                                                  g.DateArrived.CompareTo(dateTime2) <= 0);
        }

        public async Task<bool> UpdateAsync(Guid id, Code what, string userId)
        {
            Code code = await GetAsync(id, userId);
            if (code is null) return false;
            code.Update(what);
            _context.Entry(code).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(code, userId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> ContainsAsync(Code what, string userId)
        {
            return await _context.Code.AnyAsync(g => g.IsUser(userId) && g.Equals(what));
        }

        public async Task<bool> ContainsAsync(Guid id, string userId)
        {
            return await _context.Code.AnyAsync(g => g.IsUser(userId) && g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            try
            {
                _context.Code.Remove(await GetAsync(id, userId));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Code what, string userId)
        {
            Code mes = await GetAsync(what.DateArrived, userId);
            return mes.Id;
        }

        public bool AreEqual(Code m1, Code m2)
        {
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Code");
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task AddAllAsync(ISet<Code> all, string userId)
        {
            foreach(var code in all)
            {
                await AddAsync(code, userId);
            }
        }
    }
}
