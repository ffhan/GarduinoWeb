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

        public async Task<bool> AddAsync(Measure measure, Device device)
        {
            bool tmp = await IsContainedAsync(measure, device);
            measure.SetDevice(device);
            if (!tmp)
            {
                _context.Measure.Add(measure);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public IEnumerable<Measure> GetAll(Device device)
        {
            return device.Measures?.OrderByDescending(g => g.DateTime);
        }

        public async Task<Measure> GetAsync(Guid id)
        {
            return await _context.Measure.Include(c => c.Device).FirstOrDefaultAsync(g => g.Id.Equals(id));
        }

        public async Task<Measure> GetAsync(DateTime dateTime, Device device)
        {
            return device.Measures?.FirstOrDefault(g => g.DateTime.Equals(dateTime));
        }

        public async Task<IEnumerable<Measure>> GetRangeAsync(DateTime dateTime1, DateTime dateTime2, Device device)
        {
            return device.Measures?.Where(m => m.DateTime.CompareTo(dateTime1) >= 0 && m.DateTime.CompareTo(dateTime2) <= 0);
        }

        public async Task<bool> UpdateAsync(Guid id, Measure measure)
        {
            Measure mes = await GetAsync(id);
            if (mes is null) return false;
            mes.Update(measure);
            _context.Entry(mes).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsContainedAsync(Measure measure, Device device)
        {
            if (device.Measures == null) return false;
            return device.Measures.Any(g => g.EqualsEf(measure));
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
                _context.Measure.Remove(await GetAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public bool AreEqual(Measure m1, Measure m2)
        {
            return m1.Equals(m2);
        }

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Measure");
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }

        public Measure GetLatest(Device device)
        {
            return GetAll(device).FirstOrDefault();
        }

        public async Task AddAllAsync(ISet<Measure> all, Device device)
        {
            foreach (var measure in all)
            {
                await AddAsync(measure, device);
            }
        }
    }
}
