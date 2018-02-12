using System.Collections.Generic;

namespace Garduino.Models.Interfaces
{
    interface IDeviceModel : IBaseModel<Device>
    {
        string Name { get; set; }
        ICollection<Code> Code{ get; set; }
        ICollection<Measure> Measure { get; set; }
        User User { get; set; }

        void SetUser(User user);

        bool IsUser(User user);
    }
}
