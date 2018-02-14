using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GarduinoUniversal;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Garduino.Models
{
    public class Device : IDeviceModel
    {

        [Key]
        [DisplayName("ID")]
        public Guid Id { get; set; }

        [Required]
        [MinLength(4)]
        [Remote(action: "VerifyName", controller: "Device")]
        [DisplayName("Device name")]
        public string Name { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }

        [JsonIgnore]
        public virtual ICollection<Measure> Measures { get; set; }
        [JsonIgnore]
        public virtual ICollection<Code> Codes { get; set; }

        public void Update(Device code)
        {
            Name = code.Name;
        }

        public void SetUser(User user) => User = user;

        public bool IsUser(User user) => StringOperations.IsFromUser(User?.Id, user.Id);

    }
}
