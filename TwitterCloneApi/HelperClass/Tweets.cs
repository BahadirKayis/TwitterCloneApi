﻿using System;
using System.Collections.Generic;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.HelperClass
{
    public class Tweets
    {
        public Tweets()
        {
            Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public string PostContent { get; set; }
        public string PostImageUrl { get; set; }
        public int? PostLike { get; set; }
        public DateTime? Date { get; set; }
        public string? dateParse { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
