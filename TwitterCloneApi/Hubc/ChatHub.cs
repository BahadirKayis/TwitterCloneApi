
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TwitterCloneApi.Hubc
{
    [HubName("newTweetHub")]
    public class ChatHub : Hub
    {

        //SAMBAPOS5Context _db;
        //        registerterminal
        //loadterminalticket
        //updateterminalticket
        //closeterminalticket
        //unregister
        //public HubTicket(SAMBAPOS5Context db)
        //{
        //    _db = db;
        //}
       

        public  async Task tweetNotification(int id, string image_url)
        {
           

           await Clients.All.SendAsync("newTweet", id, image_url);
        }

     
   
    }
}
