using System;
using System.Collections.Generic;

#nullable disable

namespace TwitterCloneApi.Models
{
    public partial class Post
    {
        public Post()
        {
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostContent { get; set; }
        public string PostImageUrl { get; set; }
        public int? PostLike { get; set; }
        public string Date { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
