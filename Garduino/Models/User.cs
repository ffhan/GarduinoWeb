using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Garduino.Models.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Garduino.Models
{
    public class User : IUser
    {

        public User(ApplicationUser applicationUser) => ApplicationUser = applicationUser;

        public User()
        {
        }

        [Key]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Device> Device { get; set; }
    }
}
