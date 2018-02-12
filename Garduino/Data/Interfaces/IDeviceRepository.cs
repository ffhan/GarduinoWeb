using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IDeviceRepository : IBaseRepository<Device>
    {
        IEnumerable<Device> GetDevice(string device, User user);
        Task<bool> DeviceExists(string device, User user);
        bool AreEqual(Device m1, Device m2);
    }
}