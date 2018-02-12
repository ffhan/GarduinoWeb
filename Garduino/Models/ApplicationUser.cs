using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Garduino.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Device> Devices { get; set; }
    }
}
