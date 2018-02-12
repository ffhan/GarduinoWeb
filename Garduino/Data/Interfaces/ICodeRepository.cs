using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface ICodeRepository : IBaseRepository<Code>, IDeviceable<Code>, ITimeable<Code>
    {
        IEnumerable<Code> GetActive(User user);
        Task Complete(Code code, DateTime dateExecuted, User user);
        IEnumerable<Code> GetDeviceFromActiveCodes(string device, User user);
    }
}
