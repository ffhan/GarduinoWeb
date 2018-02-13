using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data.Interfaces
{
    public interface IDeviceRepository : IBaseRepository<Device, User>
    {
        Task<bool> AddAsync(Device what, User user);
        Task<Device> GetDevice(Guid device, User user);
        Task<Device> GetDevice(string name, User user);
        Task<bool> DeviceExistsAsync(string device, User user);
        bool AreEqual(Device m1, Device m2);
    }
}