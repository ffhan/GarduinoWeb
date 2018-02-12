using System.Collections.Generic;

namespace Garduino.Models.Interfaces
{
    interface IDeviceModel : IBaseModel<Device>
    {
        string Name { get; set; }
        ICollection<Code> Codes{ get; set; }
        ICollection<Measure> Measures { get; set; }
    }
}
