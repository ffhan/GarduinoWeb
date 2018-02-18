using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Data.Interfaces;
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
            code.SetDevice(device);
            try
            {
                await _context.Code.AddAsync(code);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Code> GetAll(Device device)
        {
            return device.Codes?.OrderByDescending(g => g);
        }

        public Code GetLatest(Device device)
        {
            return GetActive(device).FirstOrDefault();
        }

        public IEnumerable<Code> GetActive(Device device) => device.Codes?.Where(g => !g.IsCompleted)
            .OrderByDescending(g => g.DateArrived).Reverse();

        public async Task CompleteAsync(Code code, DateTime dateExecuted)
        {
            code.Complete(dateExecuted);
            await UpdateAsync(code.Id, code);
        }

        public async Task CompleteAsync(Guid id, DateTime dateExecuted)
        {
            Code code = await GetAsync(id);
            if (code == null) return;
            await CompleteAsync(code, dateExecuted);
        }

        public async Task<Code> GetAsync(Guid id)
        {
            return await _context.Code.Include(c => c.Device).FirstOrDefaultAsync(g => g.Id.Equals(id));
        }

        public async Task<Code> GetAsync(Code what)
        {//TODO: SWITCH TO STRINGOPERATIONS
            return await _context.Code.Include(c => c.Device).FirstOrDefaultAsync(g => g.Equals(what));
        }

        /*
        public async Task<IEnumerable<Code>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device)
        {
            return device.Codes.Where(g => g.DateArrived.CompareTo(dateTime1) >= 0 &&
                                                  g.DateArrived.CompareTo(dateTime2) <= 0);
        }*/

        public async Task<bool> UpdateAsync(Guid id, Code what)
        {
            Code code = await GetAsync(id);
            if (code is null) return false;
            code.Update(what);
            try
            {
                _context.Entry(code).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsContainedAsync(Code what, Device device)
        {
            return device.Codes.Any(g => g.Equals(what));
        }

        public async Task<bool> IsContainedAsync(Guid id, Device device)
        {
            return device.Codes.Any(g => g.Id.Equals(id));
        }

        public async Task<bool> ContainsAsync(Guid id)
        {
            return await _context.Code.AnyAsync(g => g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _context.Code.Remove(await GetAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Code");
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public async Task AddAllAsync(ISet<Code> all, Device device)
        {
            foreach(var code in all)
            {
                await AddAsync(code, device);
            }
        }
    }
}
