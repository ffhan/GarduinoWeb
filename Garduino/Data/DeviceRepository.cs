using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Data.Interfaces;
using Garduino.Hubs;
using Garduino.Models;
using GarduinoUniversal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Garduino.Data
{
    public class DeviceRepository : IDeviceRepository
        //TODO: security fixes - currently it's possible to get, & update data without being it's owner.
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<DeviceHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public void AliveEvent(object sender, PropertyChangedEventArgs p)
        {
            var dev = (Device) sender;
            _hubContext.Clients.Group(dev.User.Name).InvokeAsync("updateState", dev.Name, dev.Alive ? "has connected!" : "has died.");
            //var done = UpdateAsync(dev.Id, dev);
        }

        public DeviceRepository(IHubContext<DeviceHub> hubContext,
            ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _hubContext = hubContext;
            _context = context;
            _userManager = userManager;
        }

        public async Task<bool> AddAsync(Device what, User user)
        {
            what.SetUser(user);
            if (await DeviceExistsAsync(what.Name, user)) return false;
            try
            {
                await _context.Device.AddAsync(what);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
            return true;
        }

        public IEnumerable<Device> GetAll(User user)
        {
            var devices = _context.Device.Include(c => c.User).Include(c => c.Measures).Include(c => c.Codes).Where(g => g.IsUser(user)).ToList();
            foreach (Device device in devices)
            {
                device.PropertyChanged += AliveEvent;
                device.SetAlive();
                _context.Entry(device).State = EntityState.Modified;
            }
            _context.SaveChanges();
            return devices;
        }

        public async Task<Device> GetAsync(string name, User user)
        {
            var device = user.Devices.FirstOrDefault(g => StringOperations.IsFromDevice(g.Name, name));
            if (device == null) return null;
            device.PropertyChanged += AliveEvent;
            device.SetAlive();
            _context.Entry(device).State = EntityState.Modified;
            _context.SaveChanges();
            return device;
        }
            

        public async Task<bool> DeviceExistsAsync(string device, User user)
        {
            return await _context.Device.AnyAsync(g => g.IsUser(user) && StringOperations.IsFromDevice(g.Name, device));
        }

        public async Task<Device> GetAsync(Guid id)
        {
            var device = await _context.Device.Include(c => c.User).Include(c => c.Measures).Include(c => c.Codes).FirstOrDefaultAsync(g => g.Id.Equals(id));
            device.PropertyChanged += AliveEvent;
            return device;
        }
            

        public async Task<bool> UpdateAsync(Guid id, Device what)
        {
            var device = await GetAsync(id);
            if (device is null) return false;

            device.Update(what);
            _context.Entry(device).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }
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
            catch (DbUpdateException)
            {
                return false;
            }
            catch (ArgumentNullException)
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
            catch (DbUpdateException)
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
