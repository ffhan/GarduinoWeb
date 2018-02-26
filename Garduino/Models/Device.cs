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

        public int State { get; set; }

        [DisplayName("Global lock")]
        public bool GlobalLock => IsBitSet(0);
        [DisplayName("Logging")]
        public bool Logging => IsBitSet(1);
        [DisplayName("Written")]
        public bool Written => IsBitSet(2);
        [DisplayName("SD card initialised")]
        public bool IsInitialised=> IsBitSet(3);
        [DisplayName("Lights on")]
        public bool LightState => IsBitSet(4);
        [DisplayName("Manual light control")]
        public bool LightAdmin => IsBitSet(5);
        [DisplayName("Heating on")]
        public bool HeatState => IsBitSet(6);
        [DisplayName("Manual heating control")]
        public bool HeatAdmin => IsBitSet(7);
        [DisplayName("Watering on")]
        public bool WaterState => IsBitSet(8);
        [DisplayName("Manual watering control")]
        public bool WaterAdmin => IsBitSet(9);
        [DisplayName("Code fetched?")]
        public bool CodeFetch => IsBitSet(14);
        [DisplayName("Connectivity reconfigured?")]
        public bool NetReconf => IsBitSet(15);

        [DisplayName("Entry count")]
        public int EntryCount => Measures?.Count ?? 0;

        [DisplayName("Code count")]
        public int CodeCount => Codes?.Count ?? 0;

        [DisplayName("Last time seen")]
        public DateTime LastSign { get; set; }

        public void Update(Device code)
        {
            Name = code.Name;
            if (code.State != 0)
            {
                State = code.State;
                LastSign = DateTime.UtcNow;
            }
        }

        [DisplayName("Time since last call")]
        public TimeSpan TimeSinceSign => (DateTime.UtcNow - LastSign);

        [DisplayName("Is alive?")]
        public bool Alive => TimeSinceSign.TotalMinutes <= 2;

        public void SetUser(User user) => User = user;

        public bool IsUser(User user) => StringOperations.IsFromUser(User?.Id, user.Id);

        private bool IsBitSet(int pos)
        {
            return (State & (1 << pos)) != 0;
        }

    }
}
