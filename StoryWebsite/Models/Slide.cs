using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoryWebsite.Models
{
    public class Slide
    {
        [Key]
        public int SlideId { get; set; }
        public string ImageURL { get; set; }
        public string ImageName { get; set; }
        public string Text { get; set; }
        public int OrderNumber { get; set; }

        public int StoryId { get; set; }
        public Story Story { get; set; }
    }
}
