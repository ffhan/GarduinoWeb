using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Garduino.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Garduino.Hubs
{
    public class DeviceHub : Hub
    {

        public void NewEntry(string name)
        {
            Clients.All.InvokeAsync("newEntry", name);
        }

        public void State(string name, string message)
        {
            Clients.All.InvokeAsync("updateState", name, message);
        }

        public void FetchCode(string name, string codeName)
        {
            Clients.All.InvokeAsync("codeFetched", name, codeName);
        }

        public void DoneCode(string name, string codeName)
        {
            Clients.All.InvokeAsync("codeDone", name, codeName);
        }
    }
}
