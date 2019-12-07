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
using Microsoft.AspNetCore.Authorization;

namespace StoryWebsite.Controllers
{
    public class AppUserController : Controller
    {
        private readonly ApplicationDbContext context_;

        public AppUserController(ApplicationDbContext context)
        {
            context_ = context;
        }

        public IActionResult AppUser()
        {


            return View(context_.Users.ToList());
        }

        //[HttpGet]
        //public IActionResult Create(string id)
        //{
        //    var model = new AppUser();
        //    return View(model);
        //}

        //[HttpPost]
        //public IActionResult Create(String id, AppUser ps)
        //{
        //    context_.AppUsers.Add(ps);
        //    context_.SaveChanges();
        //    return RedirectToAction("AppUser");
        //}

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
                var person = context_.AppUsers.Find(id);
                if (person != null)
                {
                    context_.Remove(person);
                    context_.SaveChanges();
                }
            }
            catch (Exception)
            {
                // nothing for now
            }
            return RedirectToAction("AppUser");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return StatusCode(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            AppUser perosn = context_.AppUsers.Find(id);
            if (perosn == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return View(perosn);
        }

        [HttpPost]
        public IActionResult Edit(string id, AppUser ps)
        {
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            var person = context_.AppUsers.Find(id);
            if (person != null)
            {
                person.UserName = ps.UserName;
                try
                {
                    context_.SaveChanges();
                }
                catch (Exception)
                {
                    // do nothing for now
                }
            }
            return RedirectToAction("Detail", new { id = person.Id });
        }

        public ActionResult Detail(string id)
        {
            //System.Diagnostics.Debug.WriteLine(id + "&&&&&&&&&&&&&&&&&&&&&&&&&");
            if (id == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            AppUser person = context_.AppUsers.Find(id);

            if (person == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var story = context_.Stories.Where(s => s.AppUser == person);

            person.Stories = story.OrderBy(s => s.Title).Select(s => s).ToList<Story>();

            if (person.Stories == null)
            {
                person.Stories = new List<Story>();
                Story st = new Story();
                st.Title = "default";
                st.Private = "1";
                st.UserId = id;
                st.AppUser = person;
                person.Stories.Add(st);
            }
            return View(person);
        }
    }
}