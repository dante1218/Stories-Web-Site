using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StoryWebsite.Data;
using StoryWebsite.Models;

namespace StoryWebsite.Controllers
{
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext context_;

        private const string sessionId_ = "SessionId";

        public CommentController(ApplicationDbContext context)
        {
            context_ = context;
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

            var model = new Comment();
            return View(model);
        }


        [HttpPost]
        public IActionResult Add(int? id, Comment comment)
        {
            //if (id == null) return StatusCode(StatusCodes.Status400BadRequest);

            int? sid = HttpContext.Session.GetInt32(sessionId_);

            var story = context_.Stories.Find(sid);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = context_.AppUsers.Find(userId);

            if (story != null)
            {
                if (story.Comments == null)
                {
                    List<Comment> comments = new List<Comment>();
                    story.Comments = comments;
                    comment.StoryId = story.StoryId;
                    comment.Story = story;
                    comment.CreatedAt = DateTime.Now;
                    comment.AppUser = user;
                    comment.UserId = userId;
                    comment.UserName = user.UserName;
                }
                story.Comments.Add(comment);

                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }

            return RedirectToAction("View", "Slides", new { id = story.StoryId });
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
                var comment = context_.Comments.Find(id);
                storyId = comment.StoryId;
                if (comment != null)
                {
                    context_.Remove(comment);
                    context_.SaveChanges();
                }
            }
            catch (Exception)
            {
                // nothing for now
            }
            return RedirectToAction("View", "Slides", new { id = storyId });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Comment comment = context_.Comments.Find(id);
            if (comment == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(comment);
        }


        [HttpPost]
        public IActionResult Edit(int? id, Comment com)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var comment = context_.Comments.Find(id);

            if (comment != null)
            {
                comment.Title = com.Title;
                comment.Content = com.Content;
                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return RedirectToAction("View", "Slides", new { id = comment.StoryId });
        }
    }
}