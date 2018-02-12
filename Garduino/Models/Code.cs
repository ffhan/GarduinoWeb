using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Garduino.Models.ViewModels;
using GarduinoUniversal;

namespace Garduino.Models
{
    public class Code : IBaseModel<Code>
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

        [Required]
        public string DeviceName { get; set; }

        public virtual Device Device { get; set; }

        public Code(CodeViewModel codeViewModel)
        {
            ActionName = codeViewModel.ActionName;
            Action = codeViewModel.Action;
            DeviceName = codeViewModel.DeviceName;
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
            ActionName = code.ActionName;
            DeviceName = code.DeviceName;
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

        public bool IsUser(string userId) => StringOperations.IsFromUser(Device.ApplicationUser.Id, userId);

        public bool IsFromDevice(string device)
        {
            return DeviceName.ToUpper().Equals(device.ToUpper());
        }

        public Code() { }


        public int CompareTo(Code other)
        {
            if (!(IsCompleted ^ other.IsCompleted)) return -DateArrived.CompareTo(other.DateArrived);
            if (other.IsCompleted) return 1;
            return -1;
        }

        public bool EqualsEf(Code other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(Code other)
        {
            throw new NotImplementedException();
        }
    }
}
