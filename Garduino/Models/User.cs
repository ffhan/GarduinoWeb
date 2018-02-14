using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GarduinoUniversal;

namespace Garduino.Models
{
    public class User
    {
        public string Id { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public bool IsUser(string id) => StringOperations.IsFromUser(Id, id);
    }
}
