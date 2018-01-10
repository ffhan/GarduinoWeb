using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Measure measure, string userId)
        {
            measure.SetUser(userId);
            bool tmp = await ContainsAsync(measure, userId);
            if (!tmp)
            {
                _context.Measure.Add(measure);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Measure> GetAll(string userId)
        {
            return _context.Measure.Where(g => g.UserId.Equals(userId)).OrderByDescending(g => g.DateTime);
        }

        public IEnumerable<Measure> GetDevice(string device, string userId)
        {
            return _context.Measure.Where(g => g.IsFromDevice(device) && g.UserId.Equals(userId));
        }

        public async Task<Measure> GetAsync(Measure measure, string userId)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Equals(measure) && g.UserId.Equals(userId));
        }
        public async Task<Measure> GetAsync(Guid id, string userId)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Id == id && g.UserId.Equals(userId));
        }

        public async Task<Measure> GetAsync(DateTime dateTime, string userId)
        {
            var tmp = await _context.Measure.FirstOrDefaultAsync(g => g.DateTime.Equals(dateTime) && g.UserId.Equals(userId));
            return tmp;
        }

        public async Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, string userId)
        {// TODO: IMPLEMENT COMPARATOR!
            return await _context.Measure.Where(m => m.DateTime.CompareTo(dateTime1) >= 0 && m.DateTime.CompareTo(dateTime2) <= 0
            && m.UserId.Equals(userId)).ToArrayAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, Measure measure, string userId)
        {
            Measure mes = await GetAsync(id, userId);
            if (mes is null) return false;
            mes.Update(measure);
            _context.Entry(mes).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(mes, userId))
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

        public async Task<bool> ContainsAsync(Measure measure, string userId)
        {
            Measure tmp = _context.Measure.FirstOrDefault(g => g.EqualsEf(measure) && g.UserId.Equals(userId));
            return !(tmp is null);
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            try
            {
                _context.Measure.Remove(await GetAsync(id, userId));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Measure measure, string userId)
        {
            Measure mes = await GetAsync(measure.DateTime, userId);
            return mes.Id;
        }

        public bool AreEqual(Measure m1, Measure m2)
        {
            
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Measure");
            return true;
        }
    }
}
