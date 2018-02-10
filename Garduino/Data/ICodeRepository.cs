using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface ICodeRepository : IBaseRepository<Code>, IDeviceable<Code>, ITimeable<Code>
    {
        IEnumerable<Code> GetActive(string userId);
        Task Complete(Code code, DateTime dateExecuted, string userId);
        IEnumerable<Code> GetDeviceFromActiveCodes(string device, string userId);
    }
}
