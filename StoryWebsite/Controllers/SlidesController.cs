using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoryWebsite.Models;
using StoryWebsite.Data;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace StoryWebsite.Controllers
{
    public class SlidesController : Controller
    {
        private readonly ApplicationDbContext context_;

        private const string sessionId_ = "SessionId";
        private readonly IHostingEnvironment hostingEnvironment_;
        private string webRootPath = null;
        private string filePath = null;

        public SlidesController(IHostingEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            hostingEnvironment_ = hostingEnvironment;
            webRootPath = hostingEnvironment_.WebRootPath;
            filePath = Path.Combine(webRootPath, "FileStorage");
            context_ = context;
        }

        [HttpGet]
        public IActionResult View(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            Story story = context_.Stories.Find(id);

            if (story == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var slides = context_.Slides.Where(s => s.Story == story);

            story.Slides = slides.OrderBy(s => s.SlideId).Select(s => s).ToList<Slide>();

            if (story.Slides == null)
            {
                story.Slides = new List<Slide>();
                Slide st = new Slide();
                st.ImageName = "default";
                st.ImageURL = "default";
                st.StoryId = story.StoryId;
                st.Story = story;
                story.Slides.Add(st);
            }

            var comments = context_.Comments.Where(s => s.Story == story);

            story.Comments = comments.OrderBy(s => s.CreatedAt).Select(s => s).ToList<Comment>();

            if (story.Comments == null)
            {
                story.Comments = new List<Comment>();
                Comment com = new Comment();
                com.Title = "default";
                com.Content = "default";
                com.CreatedAt = DateTime.Now;
                com.StoryId = story.StoryId;
                com.Story = story;
                story.Comments.Add(com);
            }

            return View(story);
        }

        [HttpGet]
        public IActionResult Add(int id)
        {
            HttpContext.Session.SetInt32(sessionId_, id);

            Story story = context_.Stories.Find(id);
            if (story == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var model = new Slide();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Slide slide, IFormFile image, string text)
        {
            string imageName = null;

            if (image != null)
            {
                imageName = image.FileName;

                var path = Path.Combine(filePath, imageName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                slide.ImageName = imageName;
                slide.ImageURL = filePath;
            }

            if (text != null)
            {
                slide.Text = text;
            }

            int? sid = HttpContext.Session.GetInt32(sessionId_);

            var story = context_.Stories.Find(sid);

            if (story != null)
            {
                if (story.Slides == null)
                {
                    List<Slide> slides = new List<Slide>();
                    story.Slides = slides;
                    slide.StoryId = story.StoryId;
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

            return RedirectToAction("View", new { id = story.StoryId });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Slide slide = context_.Slides.Find(id);
            if (slide == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(slide);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, IFormFile image, string text)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            Slide slide = context_.Slides.Find(id);

            if (slide != null)
            {
                string imageName = null;

                if (image != null)
                {
                    imageName = image.FileName;

                    var path = Path.Combine(filePath, imageName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await image.CopyToAsync(fileStream);
                    }

                    slide.ImageName = imageName;
                    slide.ImageURL = filePath;
                }

                if (text != null)
                {
                    //var path = Path.Combine(filePath, text.FileName);

                    //using (var fileStream = new FileStream(path, FileMode.Create))
                    //{
                    //    await text.CopyToAsync(fileStream);
                    //}

                    //string content = System.IO.File.ReadAllText(path);
                    //slide.Text = content;
                    slide.Text = text;
                }

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }

            return RedirectToAction("View", new { id = slide.StoryId });
        }

        public IActionResult Delete(int? id)
        {
            int storyId = 0;
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
                var slide = context_.Slides.Find(id);
                storyId = slide.StoryId;
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
            return RedirectToAction("View", new { id = storyId });
        }
    }
}