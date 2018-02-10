using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Data
{
    public interface IDeviceable<T>
    {
        IEnumerable<T> GetDevice(string device, string userId);
    }
}
