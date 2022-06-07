using System;
using System.Collections.Generic;

#nullable disable

namespace TwitterCloneApi.Models
{
    public partial class Tag
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public string TagName { get; set; }
        public DateTime? Date { get; set; }

        public virtual Post Post { get; set; }
    }
}
