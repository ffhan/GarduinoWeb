using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IDeviceRepository : IBaseRepository<Device, ApplicationUser>
    {
        Task<bool> AddAsync(Device what, ApplicationUser user);
        IEnumerable<Device> GetDevice(string device, ApplicationUser user);
        Task<bool> DeviceExistsAsync(string device, ApplicationUser user);
        bool AreEqual(Device m1, Device m2);
    }
}