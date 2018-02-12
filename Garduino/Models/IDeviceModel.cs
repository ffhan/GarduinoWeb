using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models
{
    interface IDeviceModel : IBaseModel<Device>
    {
        void SetUser(ApplicationUser user);
        bool IsUser(ApplicationUser user);
    }
}
