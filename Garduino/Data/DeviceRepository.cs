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
            return _context.Device.Where(g => g.IsUser(user));
        }

        public IEnumerable<Device> GetDevice(string device, User user)
        {
            return _context.Device.Where(g => g.Name.Equals(device) && g.IsUser(user));
        }

        public async Task<bool> DeviceExists(string device, User user)
        {
            if (user.Device == null) return false;
            foreach (var dev in user.Device)
            {
                if(StringOperations.IsFromDevice(dev.Name, device)) return true;
            }
            return false;
        }

            public async Task<Device> GetAsync(Guid id, User user)
        {
            return await _context.Device.FirstOrDefaultAsync(g => g.Id.Equals(id) && g.IsUser(user));
        }

        public async Task<Device> GetAsync(Device what, User user)
        {
            return await _context.Device.FirstOrDefaultAsync(g => g.Equals(what) && g.IsUser(user));
        }

        public async Task<bool> UpdateAsync(Guid id, Device what, User user)
        {
            Device device = await GetAsync(id, user);
            if (device is null) return false;
            device.Update(what);
            _context.Entry(device).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ContainsAsync(device, user))
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

        public async Task<bool> ContainsAsync(Device what, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && Equals(what));
        }

        public async Task<bool> ContainsAsync(Guid id, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && g.Id.Equals(id));
        }

        public async Task<bool> DeleteAsync(Guid id, User user)
        {
            try
            {
                _context.Device.Remove(await GetAsync(id, user));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            return true;
        }

        public async Task<Guid> GetId(Device what, User user)
        {
            Device device = await _context.Device.FirstOrDefaultAsync(g => g.IsUser(user) && Equals(what));
            return device.Id;
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
