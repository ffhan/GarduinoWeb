using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IDeviceRepository : IBaseRepository<Device>
    {
        IEnumerable<Device> GetDevice(string device, string userId);
        Task<bool> DeviceExists(string device, string userId);
        bool AreEqual(Device m1, Device m2);
    }
}