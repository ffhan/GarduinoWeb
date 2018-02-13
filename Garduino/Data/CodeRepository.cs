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

        public IEnumerable<Code> GetAll(Device device)
        {//TODO: make sense of this
            return device.Codes;
        }

        public Code GetLatest(Device device)
        {
            return device.Codes.OrderByDescending(g => g).Reverse().FirstOrDefault();
        }

        public IEnumerable<Code> GetActive(Device device) => device.Codes.Where(g => !g.IsCompleted);

        public async Task Complete(Code code, DateTime dateExecuted, Device device)
        {
            code.Complete(dateExecuted);
            await UpdateAsync(code.Id, code, device);
        }

        public async Task Complete(Guid id, DateTime dateExecuted, Device device)
        {
            Code code = await GetAsync(id, device);
            code.Complete(dateExecuted);
            await UpdateAsync(id, code, device);
        }

        public IEnumerable<Code> GetActiveCodesFromDevice(Device device)
        {
            return device.Codes.Where(g => !g.IsCompleted);
        }

        public async Task<Code> GetAsync(Guid id, Device device)
        {
            return device.Codes.FirstOrDefault(g => g.Id.Equals(id));
        }

        public async Task<Code> GetAsync(DateTime dateTime, Device device)
        {
            return device.Codes.FirstOrDefault(g => g.DateArrived.Equals(dateTime));
        }

        public async Task<Code> GetAsync(Code what, Device device)
        {//TODO: SWITCH TO STRINGOPERATIONS
            return device.Codes.FirstOrDefault(g => g.Equals(what));
        }

        public async Task<IEnumerable<Code>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device)
        {
            return device.Codes.Where(g => g.DateArrived.CompareTo(dateTime1) >= 0 &&
                                                  g.DateArrived.CompareTo(dateTime2) <= 0);
        }

        public async Task<bool> UpdateAsync(Guid id, Code what, Device device)
        {
            Code code = await GetAsync(id, device);
            if (code is null) return false;
            code.Update(what);
            _context.Entry(code).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(code, device))
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

        public async Task<bool> ContainsAsync(Code what, Device device)
        {
            return device.Codes.Any(g => g.Equals(what));
        }

        public async Task<bool> ContainsAsync(Guid id, Device device)
        {
            return device.Codes.Any(g => g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, Device device)
        {
            try
            {
                _context.Code.Remove(await GetAsync(id, device));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Code what, Device device)
        {
            Code mes = await GetAsync(what.DateArrived, device);
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

        public async Task AddAllAsync(ISet<Code> all, Device device)
        {
            foreach(var code in all)
            {
                await AddAsync(code, device);
            }
        }
    }
}
