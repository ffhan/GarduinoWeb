using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarduinoAPI.Models;

namespace GarduinoAPI.Data
{
    public class EntryRepository : IEntryRepository
    {
        private readonly ApplicationDbContext _context;

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Measure measure)
        {
            bool tmp = await ContainsAsync(measure);
            if (!tmp)
            {
                _context.Measure.Add(measure);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Measure> GetAll()
        {
            return _context.Measure.OrderByDescending(g => g.DateTime);
        }

        public async Task<Measure> GetAsync(Measure measure)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Equals(measure));
        }
        public async Task<Measure> GetAsync(Guid id)
        {
            return await _context.Measure.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<Measure> GetAsync(DateTime dateTime)
        {
            var tmp = await _context.Measure.FirstOrDefaultAsync(g => g.DateTime.Equals(dateTime));
            return tmp;
        }

        public async Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2)
        {// TODO: IMPLEMENT COMPARATOR!
            return await _context.Measure.Where(m => m.DateTime.CompareTo(dateTime1) >= 0 && m.DateTime.CompareTo(dateTime2) <= 0).ToArrayAsync();
        }

        public async Task<bool> UpdateAsync(Guid id, Measure measure)
        {
            Measure mes = await GetAsync(id);
            if (mes is null) return false;
            mes.Update(measure);
            _context.Entry(mes).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(mes))
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

        public async Task<bool> ContainsAsync(Measure measure)
        {
            Measure tmp = _context.Measure.FirstOrDefault(g => g.EqualsEf(measure));
            return !(tmp is null);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            _context.Measure.Remove(await GetAsync(id));
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Measure measure)
        {
            Measure mes = await GetAsync(measure.DateTime);
            return mes.Id;
        }

        public bool AreEqual(Measure m1, Measure m2)
        {
            
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            int tmp = await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Measure");
            if (tmp == 0) return true;
            return false;
        }
    }
}
