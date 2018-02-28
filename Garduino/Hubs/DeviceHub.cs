using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Garduino.Models;
using Microsoft.AspNetCore.Authorization;
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

        private IIdentity GetIdentity() => (ClaimsIdentity) Context.User.Identity;
        

        public override Task OnConnectedAsync()
        {
            var ident = GetIdentity(); //Send only to this user.
            Groups.AddAsync(Context.ConnectionId, ident.Name);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var ident = GetIdentity();
            Groups.RemoveAsync(Context.ConnectionId, ident.Name);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
