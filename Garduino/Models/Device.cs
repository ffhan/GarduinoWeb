using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Hubs;
using GarduinoUniversal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;

namespace Garduino.Models
{
    public class Device : IDeviceModel, INotifyPropertyChanged
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
        public virtual ICollection<Entry> Measures { get; set; }
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

        [DisplayName("Last time (UTC) seen")]
        public DateTime LastSign { get; set; }

        [DisplayName("Last time (local) seen")]
        public DateTime LocalLastSign => User.ConvertTime(LastSign);



        public void Update(Device code)
        {
            Name = code.Name;
            //_alive = code.Alive;
            SetAlive();
            
            if (code.State != 0)
            {
                State = code.State;
                LastSign = DateTime.UtcNow;
                
                //SetAlive();
            }
        }

        [DisplayName("Time since last call")]
        public TimeSpan TimeSinceSign => (DateTime.UtcNow - LastSign);

        public bool _alive { get; set; }

        [DisplayName("Is alive?")]
        public bool Alive
        {
            get
            {
                bool isAl = IsAlive();
                if (_alive && !isAl)
                {
                    
                }
                if (_alive != isAl)
                {
                    _alive = isAl;
                    OnPropertyChanged(); //TODO: see https://msdn.microsoft.com/en-us/library/xwbwks95(v=vs.100).aspx
                }
                return _alive;
            }
        }

        public void SetAlive()
        {
            var b = Alive;
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        

        private bool IsAlive() => TimeSinceSign.TotalMinutes < 2;

        public void SetUser(User user) => User = user;

        public bool IsUser(User user) => StringOperations.IsFromUser(User?.Id, user.Id);

        private bool IsBitSet(int pos)
        {
            return (State & (1 << pos)) != 0;
        }

    }
}
