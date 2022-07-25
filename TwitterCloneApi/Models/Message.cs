using System;
using System.Collections.Generic;

#nullable disable

namespace TwitterCloneApi.Models
{
    public partial class Message
    {
        public int Id { get; set; }
        public int SendUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public string MessageContent { get; set; }
        public DateTime? Date { get; set; }
    }
}
