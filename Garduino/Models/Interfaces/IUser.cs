using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models.Interfaces
{
    public interface IUser
    {
        string Id { get; set; }
        ApplicationUser ApplicationUser { get; set; }
        ICollection<Device> Device { get; set; }
    }
}
