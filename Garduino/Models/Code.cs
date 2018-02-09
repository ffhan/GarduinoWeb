using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Garduino.Models.ViewModels;

namespace Garduino.Models
{
    public class Code : IComparable<Code>
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

        [HiddenInput]
        public string UserId { get; set; }

        [Required]
        public string DeviceName { get; set; }

        public Code(CodeViewModel codeViewModel)
        {
            ActionName = codeViewModel.ActionName;
            Action = codeViewModel.Action;
            DeviceName = codeViewModel.DeviceName;
        }

        public void SetUser(string id) => UserId = id;

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
    }
}
