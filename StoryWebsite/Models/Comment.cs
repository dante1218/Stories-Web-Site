using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoryWebsite.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public AppUser AppUser { get; set; }

        public int StoryId { get; set; }
        public Story Story { get; set; }
    }
}
