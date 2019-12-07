using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using StoryWebsite.Models;
using StoryWebsite.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StoryWebsite
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string filePath = null;
        private readonly ApplicationDbContext context_;

        public FileController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            filePath = Path.Combine(webRootPath, "FileStorage");
            context_ = context;
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
          {
            {".cs", "application/C#" },
            {".txt", "text/plain"},
            {".pdf", "application/pdf"},
            {".doc", "application/vnd.ms-word"},
            {".docx", "application/vnd.ms-word"},
            {".xls", "application/vnd.ms-excel"},
            {".xlsx", "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet"},
            {".png", "image/png"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".gif", "image/gif"},
            {".csv", "text/csv"}
          };
        }

        [HttpPost("uploadStory")]
        public IActionResult UploadStory()
        {
            string userId = Request.Form["id"];
            string Title = Request.Form["title"];
            string Private = Request.Form["private"];
            if (Private != "0" && Private != "1") Private = "0";

            Story story = new Story();
            story.Title = Title;
            story.Private = Private;

            var person = context_.AppUsers.Find(userId);

            if (person != null)
            {
                if (person.Stories == null)
                {
                    List<Story> stories = new List<Story>();
                    person.Stories = stories;
                    story.UserId = userId;
                    story.AppUser = person;
                }
                person.Stories.Add(story);

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("replaceStory")]
        public IActionResult ReplaceStory()
        {
            int storyId = Convert.ToInt32(Request.Form["id"]);
            string Title = Request.Form["title"];
            string Private = Request.Form["private"];
            if (Private != "0" && Private != "1") Private = "0";

            Story story = context_.Stories.Find(storyId);

            if (story != null)
            {
                if (Title != null && Title != "") story.Title = Title;
                if (Private != null && Private != "") story.Private = Private;
            }
            else
            {
                return BadRequest();
            }
            try
            {
                context_.SaveChanges();
            }
            catch (Exception)
            {
                // do nothing for now
            }

            return Ok();
        }

        // POST api/<controller>/upload
        [HttpPost("uploadSlide")]
        public async Task<IActionResult> UploadSlide()
        {
            int storyId = Convert.ToInt32(Request.Form["story"]);
            string textContent = Request.Form["textContent"];

            Slide slide = new Slide();
            if (textContent != null) slide.Text = textContent;

            var request = HttpContext.Request;

            foreach (var file in request.Form.Files){
                if (file.Length > 0)
                {
                    var path = Path.Combine(filePath, file.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    string type = GetContentType(path);
                    System.Diagnostics.Debug.WriteLine(storyId + "=======================================================================");

                    if (type == "image/png" || type == "image/jpeg" || type == "image/jpeg" || type == "image/gif")
                    {
                        slide.ImageURL = filePath;
                        slide.ImageName = file.FileName;
                    }
                }
            }
            slide.OrderNumber = 0;
            var story = context_.Stories.Find(storyId);
            if (story != null){
                if (story.Slides == null){
                    List<Slide> slides = new List<Slide>();
                    story.Slides = slides;
                    slide.StoryId = storyId;
                    slide.Story = story;
                }
                story.Slides.Add(slide);

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return Ok();
        }

        // POST api/<controller>/replace
        [HttpPost("replaceSlide")]
        public async Task<IActionResult> ReplaceSlide()
        {
            int slideId = Convert.ToInt32(Request.Form["slide"]);
            string textContent = Request.Form["textContent"];

            Slide slide = context_.Slides.Find(slideId);
            if (slide == null) return BadRequest();

            if (textContent != null && textContent != "") slide.Text = textContent;

            var request = HttpContext.Request;

            foreach (var file in request.Form.Files)
            {
                if (file.Length > 0)
                {
                    var path = Path.Combine(filePath, file.FileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    string type = GetContentType(path);
                    System.Diagnostics.Debug.WriteLine(slideId + "=======================================================================");


                    if (type == "image/png" || type == "image/jpeg" || type == "image/jpeg" || type == "image/gif")
                    {
                        slide.ImageURL = filePath;
                        slide.ImageName = file.FileName;
                    }
                }
            }

            try
            {
                context_.SaveChanges();
            }
            catch (Exception)
            {
                // do nothing for now
            }

            return Ok();
        }

        // DELETE api/<controller>/5
        [HttpDelete("story={id}")]
        public IActionResult DeleteStory(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                var story = context_.Stories.Find(id);
                if (story != null)
                {
                    context_.Remove(story);
                    context_.SaveChanges();
                }
            }
            catch (Exception)
            {
                // nothing for now
            }

            return Ok();
        }

        // DELETE api/<controller>/5
        [HttpDelete("slide={id}")]
        public IActionResult DeleteSlide(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            try
            {
                var slide = context_.Slides.Find(id);
                if (slide != null)
                {
                    context_.Remove(slide);
                    context_.SaveChanges();
                }
            }
            catch (Exception)
            {
                // nothing for now
            }

            return Ok();
        }
    }
}
