using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [Required]
        [MinLength(4)]
        [Remote(action: "VerifyName", controller: "Device")]
        [DisplayName("Device name")]
        public string Name { get; set; }

        public ApplicationUser User { get; set; }
        public void SetUser(ApplicationUser user)
        {
            User = user;
        }

        public ICollection<Measure> Measures { get; set; }

        public ICollection<Code> Codes { get; set; }

        public void Update(Device code)
        {
            Name = code.Name;
        }

        public bool IsUser(ApplicationUser user) => StringOperations.IsFromUser(User.Id, user.Id);

        public int CompareTo(Device other) => String.Compare(Name, other.Name, StringComparison.Ordinal);

    }
}
