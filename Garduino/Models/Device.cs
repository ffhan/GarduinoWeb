using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Measure> Measures { get; set; }

        public virtual ICollection<Code> Codes { get; set; }

        public void Update(Device code)
        {
            Name = code.Name;
        }

        public void SetUser(ApplicationUser user) => ApplicationUser = user;

        public bool IsUser(ApplicationUser user) => StringOperations.IsFromUser(ApplicationUser.Id, user.Id);

    }
}
