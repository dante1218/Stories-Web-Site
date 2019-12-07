using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoryWebsite.Models
{
    public class Story
    {
        [Key]
        public int StoryId { get; set; }
        public string Title { get; set; }
        public string Private { get; set; }

        public string UserId { get; set; }
        public AppUser AppUser { get; set; }

        public ICollection<Slide> Slides { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
