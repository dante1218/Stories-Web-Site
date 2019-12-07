using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoryWebsite.Models
{
    public class AppUser : IdentityUser
    {
        public ICollection<Story> Stories { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
