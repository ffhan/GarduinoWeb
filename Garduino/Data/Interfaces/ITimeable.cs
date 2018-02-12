using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface ITimeable<T>
    {
        T GetLatest(User user);
    }
}
