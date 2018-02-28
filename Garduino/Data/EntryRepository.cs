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
    public class EntryRepository : IMeasureRepository
    {
        private readonly ApplicationDbContext _context;

        public EntryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Entry entry, Device device)
        {
            bool tmp = await IsContainedAsync(entry, device);
            entry.SetDevice(device);
            if (tmp) return false;
            try
            {
                _context.Measure.Add(entry);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Entry> GetAll(Device device)
        {
            return device.Measures?.OrderByDescending(g => g.DateTime);
        }

        public async Task<Entry> GetAsync(Guid id)
        {
            return await _context.Measure.Include(c => c.Device).Include(c => c.Device.User).FirstOrDefaultAsync(g => g.Id.Equals(id));
        }

        public async Task<Entry> GetAsync(DateTime dateTime, Device device)
        {
            return device.Measures?.FirstOrDefault(g => g.DateTime.Equals(dateTime));
        }

        public async Task<IEnumerable<Entry>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device)
        {
            return device.Measures?.Where(m => m.DateTime.CompareTo(dateTime1) >= 0 && m.DateTime.CompareTo(dateTime2) <= 0);
        }

        public async Task<bool> UpdateAsync(Guid id, Entry entry)
        {
            Entry mes = await GetAsync(id);
            if (mes is null) return false;
            try
            {
                mes.Update(entry);
                _context.Entry(mes).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> IsContainedAsync(Entry entry, Device device)
        {
            if (device.Measures == null) return false;
            return device.Measures.Any(g => g.EqualsEf(entry));
        }

        public async Task<bool> IsContainedAsync(Guid id, Device device)
        {
            if (device.Measures == null) return false;
            return device.Measures.Any(g => StringOperations.IsFromDevice(g.Device.Name, device.Name) && g.Id.Equals(id));
        }

        public async Task<bool> ContainsAsync(Guid id)
        {
            return await _context.Measure.AnyAsync(g => g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                Entry mes = await GetAsync(id);
                if (mes == null) return false;
                _context.Measure.Remove(mes);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public bool AreEqual(Entry m1, Entry m2)
        {
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Entry");
            }
            catch (DbUpdateException)
            {
                return false;
            }
            
            return true;
        }

        public Entry GetLatest(Device device)
        {
            return GetAll(device).FirstOrDefault();
        }

        public async Task AddAllAsync(ISet<Entry> all, Device device)
        {
            foreach (var measure in all)
            {
                await AddAsync(measure, device);
            }
        }
    }
}
