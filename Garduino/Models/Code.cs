using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Garduino.Models.ViewModels;
using GarduinoUniversal;
using Newtonsoft.Json;

namespace Garduino.Models
{
    public class Code : ICodeModel
    {
        [Key]
        [DisplayName("ID")]
        public Guid Id { get; set; }

        [Required]
        [DisplayName("Action")]
        public int Action { get; set; }

        [DisplayName("Action name")]
        [DefaultValue("No name")]
        public string ActionName { get; set; }

        [DisplayName("Date arrived")]
        public DateTime DateArrived { get; set; }

        [DisplayName("Date completed")]
        public DateTime DateCompleted { get; set; }

        [DisplayName("Date executed")]
        public DateTime DateExecuted { get; set; }

        [DisplayName("Is it completed?")]
        public bool IsCompleted { get; set; }

        [JsonIgnore]
        public virtual Device Device { get; set; }

        public Code(CodeViewModel codeViewModel)
        {
            ActionName = codeViewModel.ActionName;
            Action = codeViewModel.Action;
        }

        public Code(Code other)
        {
            Action = other.Action;
            ActionName = other.ActionName;
        }


        public void Complete(DateTime dateExecuted)
        {
            IsCompleted = true;
            DateExecuted = dateExecuted;
            DateCompleted = DateTime.Now;
        }

        public void Update(Code code)
        {
            Action = code.Action;
            if (IsCompleted != code.IsCompleted && code.IsCompleted)
            {
                DateCompleted = DateTime.Now;
                IsCompleted = code.IsCompleted;
            }
            else if (IsCompleted != code.IsCompleted && !code.IsCompleted)
            {
                DateCompleted = DateTime.MinValue;
                IsCompleted = code.IsCompleted;
            }
        }

        public bool IsFromDevice(Device device) => StringOperations.IsFromDevice(Device.Name, device.Name);
        //TODO: see if it's smart to do this with name instead of id

        public Code() { }


        public int CompareTo(Code other)
        {
            if (!(IsCompleted ^ other.IsCompleted)) return -DateArrived.CompareTo(other.DateArrived);
            if (other.IsCompleted) return 1;
            return -1;
        }

        public void SetDevice(Device device)
        {
            Device = device;
        }
    }
}
