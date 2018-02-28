using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Chart.Mvc.Extensions;
using GarduinoUniversal;

namespace Garduino.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        [DisplayName("Your Time Zone:")]
        public string TimeZone { get; set; }

        public virtual ICollection<Device> Devices { get; set; }

        public bool IsUser(string id) => StringOperations.IsFromUser(Id, id);

        public void Update(User user)
        {
            Name = user.Name;
            TimeZone = user.TimeZone;
        }

        public DateTime ConvertTime(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.FindSystemTimeZoneById(TimeZone ?? TimeZoneInfo.Utc.Id));
        }

        public DateTime GetUserTime() => ConvertTime(DateTime.UtcNow);

    }
}
