using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using StoryWebsite.Data;
using StoryWebsite.Models;

namespace StoryWebsite.Controllers
{
    public class StoryController : Controller
    {
        private readonly ApplicationDbContext context_;

        private const string sessionId_ = "SessionId";

        public StoryController(ApplicationDbContext context)
        {
            context_ = context;
        }

        public IActionResult Story()
        {
            var storys = context_.Stories.Include(s => s.AppUser);
            var orderedStorys = storys.OrderBy(s => s.Title)
              .OrderBy(l => l.AppUser)
              .Select(l => l);
            return View(orderedStorys);
        }

        [HttpGet]
        public IActionResult Create(string id)
        {
            HttpContext.Session.SetString(sessionId_, id);

            AppUser per = context_.AppUsers.Find(id);
            if (per == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }

            var model = new Story();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(int? id, Story story)
        {
            //if (id == null) return StatusCode(StatusCodes.Status400BadRequest);

            string pid = HttpContext.Session.GetString(sessionId_);

            var person = context_.AppUsers.Find(pid);

            if (person != null)
            {
                if (person.Stories == null)
                {
                    List<Story> stories = new List<Story>();
                    person.Stories = stories;
                    story.UserId = pid;
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

            return RedirectToAction("Detail", "AppUser", new { id = pid });
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
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
            return RedirectToAction("Story");
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Story story = context_.Stories.Find(id);
            if (story == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(story);
        }


        [HttpPost]
        public IActionResult Edit(int? id, Story st)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var story = context_.Stories.Find(id);

            if (story != null)
            {
                story.Title = st.Title;
                story.Private = st.Private;
                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return RedirectToAction("Detail", new { id = story.StoryId });
        }

        public ActionResult Detail(int? id)
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

            return View(story);
        }
    }
}