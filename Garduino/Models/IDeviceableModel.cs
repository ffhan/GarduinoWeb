using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    public interface IDeviceableModel
    {
        Device Device { get; set; }

        void SetDevice(Device device);

        bool IsFromDevice(Device device);
    }
}
