using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Data.Interfaces;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Garduino.Data
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context) => _context = context;

        public async Task<bool> AddAsync(Device what, User user)
        {
            what.SetUser(user);
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

        public IEnumerable<Device> GetAll(User user)
        {
            return _context.Device.Include(c => c.User).Include(c => c.Measures).Include(c => c.Codes).Where(g => g.IsUser(user));
        }

        public async Task<Device> GetAsync(string name, User user) =>
            user.Devices.FirstOrDefault(g => StringOperations.IsFromDevice(g.Name, name));

        public async Task<bool> DeviceExistsAsync(string device, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && StringOperations.IsFromDevice(g.Name, device));
        }

        public async Task<Device> GetAsync(Guid id) => 
            await _context.Device.Include(c => c.User).Include(c => c.Measures).Include(c => c.Codes).FirstOrDefaultAsync(g => g.Id.Equals(id));

        public async Task<bool> UpdateAsync(Guid id, Device what)
        {
            var device = await GetAsync(id);
            if (device is null) return false;
            device.Update(what);
            _context.Entry(device).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsContainedAsync(Device what, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && g.Equals(what));
        }

        public async Task<bool> IsContainedAsync(Guid id, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && g.Id.Equals(id));
        }

        public async Task<bool> ContainsAsync(Guid id)
        {
            return await _context.Device.AnyAsync(g => g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                _context.Device.Remove(await GetAsync(id));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public bool AreEqual(Device m1, Device m2) => m1.Equals(m2);

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

        public async Task AddAllAsync(ISet<Device> all, User user)
        {
            foreach (Device device in all)
            {
                await AddAsync(device, user);
            }
        }
    }
}
