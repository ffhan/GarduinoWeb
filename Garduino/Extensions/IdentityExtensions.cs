using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Garduino.Models;

namespace Garduino.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetOrganizationId(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("Device");
            // Test for null to avoid issues during local testing
            return claim.Value ?? string.Empty;
        }
    }
}
