using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterCloneApi.Hubss
{
    [HubName("newTweetHub")]
    public class NewTweetHub : Hub
    {
        public async Task newTweet(string id, string image_url)
        {


            await Clients.All.SendAsync("newTweet", id, image_url);
        }

    }
}
