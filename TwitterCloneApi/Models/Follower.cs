using System;
using System.Collections.Generic;

#nullable disable

namespace TwitterCloneApi.Models
{
    public partial class Follower
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int Followed { get; set; }
        public DateTime? Date { get; set; }

        public virtual Post FollowedNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
