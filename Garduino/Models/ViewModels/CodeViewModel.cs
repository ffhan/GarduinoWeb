using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garduino.Models.ViewModels
{
    public class CodeViewModel
    {
        [DisplayName("Action name")]
        public int Action { get; set; }
        
        [DisplayName("Action name")]
        [DefaultValue("No name")]
        public string ActionName { get; set; }

        [DisplayName("Device name")]
        [DefaultValue("No name")]
        public string DeviceName { get; set; }
    }
}
