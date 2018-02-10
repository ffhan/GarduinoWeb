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
    public class Device : IBaseModel<Device>
    {

        [Key]
        [DisplayName("ID")]
        public Guid Id { get; set; }

        [Required]
        [MinLength(4)]
        [Remote(action: "VerifyName", controller: "Device")]
        [DisplayName("Device name")]
        public string Name { get; set; }

        [HiddenInput]
        public string UserId { get; set; }

        public virtual ICollection<Measure> Measures { get; set; }

        public virtual ICollection<Code> Codes { get; set; }

        public void SetUser(string id) => UserId = id;
        public void Update(Device code)
        {
            Name = code.Name;
        }

        public bool EqualsEf(Device other)
        {
            return Equals(other);
        }

        public bool IsUser(string userId) => StringOperations.IsFromUser(UserId, userId);

        public int CompareTo(Device other) => String.Compare(Name, other.Name, StringComparison.Ordinal);

    }
}
