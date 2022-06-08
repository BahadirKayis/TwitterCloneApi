using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterCloneApi.Hubss
{
  
    public class NewPostHub : Hub
    {
        public async System.Threading.Tasks.Task newPost(string name, string message)
        {


            await Clients.All.SendAsync("newPost", name, message);
        }

    }
}
