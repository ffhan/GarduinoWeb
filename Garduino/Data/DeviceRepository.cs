using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.EntityFrameworkCore;

namespace Garduino.Data
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context) => _context = context;

        public async Task<bool> AddAsync(Device what, string userId)
        {

            try
            {
                await _context.Device.AddAsync(what);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Device> GetAll(string userId)
        {
            return _context.Device.Where(g => g.IsUser(userId));
        }

        public IEnumerable<Device> GetDevice(string device, string userId)
        {
            return _context.Device.Where(g => g.Name.Equals(device) && g.IsUser(userId));
        }

        public async Task<bool> DeviceExists(string device, string userId)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(userId) && StringOperations.IsFromDevice(g.Name, device));
        }

        public async Task<Device> GetAsync(Guid id, string userId)
        {
            return await _context.Device.FirstOrDefaultAsync(g => g.Id.Equals(id) && g.IsUser(userId));
        }

        public async Task<Device> GetAsync(Device what, string userId)
        {
            return await _context.Device.FirstOrDefaultAsync(g => g.Equals(what) && g.IsUser(userId));
        }

        public async Task<bool> UpdateAsync(Guid id, Device what, string userId)
        {
            Device device = await GetAsync(id, userId);
            if (device is null) return false;
            device.Update(what);
            _context.Entry(device).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(device, userId))
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

        public async Task<bool> ContainsAsync(Device what, string userId)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(userId) && Equals(what));
        }

        public async Task<bool> ContainsAsync(Guid id, string userId)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(userId) && g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, string userId)
        {
            try
            {
                _context.Device.Remove(await GetAsync(id, userId));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Device what, string userId)
        {
            Device device = await _context.Device.FirstOrDefaultAsync(g => g.IsUser(userId) && Equals(what));
            return device.Id;
        }

        public bool AreEqual(Device m1, Device m2) => m1.EqualsEf(m2);

        public async Task<bool> DeleteAllAsync()
        {
            try
            {
                await _context.Database.ExecuteSqlCommandAsync("TRUNCATE TABLE Device");
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task AddAllAsync(ISet<Device> all, string userId)
        {
            foreach (Device device in all)
            {
                await AddAsync(device, userId);
            }
        }
    }
}
