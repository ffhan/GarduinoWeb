﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Data
{
    public interface IDeviceable<T>
    {
        IEnumerable<T> GetDevice(string device, string userId);
        Task<bool> AddAsync(T what, Device device);
    }
}
