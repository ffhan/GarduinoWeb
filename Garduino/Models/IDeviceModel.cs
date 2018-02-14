using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    interface IDeviceModel : IBaseModel<Device>
    {
        void SetUser(User user);
        bool IsUser(User user);
    }
}
