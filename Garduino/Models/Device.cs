using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models.Interfaces;
using GarduinoUniversal;
using Microsoft.AspNetCore.Mvc;

namespace Garduino.Models
{
    public class Device : IDeviceModel
    {

        [Key]
        [DisplayName("ID")]
        public Guid Id { get; set; }

        public virtual User User { get; set; }

        [Required]
        [MinLength(4)]
        [Remote(action: "VerifyName", controller: "Device")]
        [DisplayName("Device name")]
        public string Name { get; set; }

        public void SetUser(User user)
        {
            User = user;
        }

        public virtual ICollection<Measure> Measure { get; set; }

        public virtual ICollection<Code> Code { get; set; }

        public void Update(Device code)
        {
            Name = code.Name;
        }

        public bool IsUser(User user) => StringOperations.IsFromUser(User.Id, user.Id);

        public int CompareTo(Device other) => string.Compare(Name, other.Name, StringComparison.Ordinal);

    }
}
