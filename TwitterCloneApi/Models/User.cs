using System;
using System.Collections.Generic;

#nullable disable

namespace TwitterCloneApi.Models
{
    public partial class User
    {
        public User()
        {
            Followers = new HashSet<Follower>();
            Messages = new HashSet<Message>();
            Posts = new HashSet<Post>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserPassword { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime? Date { get; set; }

        public virtual ICollection<Follower> Followers { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}
