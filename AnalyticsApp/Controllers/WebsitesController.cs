﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnalyticsApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace AnalyticsApp.Controllers
{
    [Produces("application/json")]
    [Route("api/websites")]
    [Authorize]
    public class WebsitesController : Controller
    {
        private readonly AnalyticsAppContext _context;
        private readonly UserManager<User> _userManager;

        public WebsitesController(AnalyticsAppContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/websites
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetWebsite()
        {
            var user = await _context.User.Include("Websites").SingleOrDefaultAsync(m => m.Id == User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            return Json(user.Websites);
        }

        // GET: api/websites/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetWebsite(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var website = await _context.Website.SingleOrDefaultAsync(m => m.WebsiteId == id);

            if (website == null)
            {
                return NotFound();
            }

            return Ok(website);
        }

        // PUT: api/websites/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWebsite([FromRoute] Guid id, [FromBody] Website website)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != website.WebsiteId)
            {
                return BadRequest();
            }

            _context.Entry(website).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WebsiteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/websites
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostWebsite([FromBody] Website website)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == User.Identity.Name);

            if (user == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user.Websites.Add(website);
            _context.Website.Add(website);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWebsite", new { id = website.WebsiteId }, website);
        }

        // DELETE: api/websites/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteWebsite(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var website = await _context.Website.SingleOrDefaultAsync(m => m.WebsiteId == id);
            if (website == null)
            {
                return NotFound();
            }

            _context.Website.Remove(website);
            await _context.SaveChangesAsync();

            return Ok(website);
        }

        private bool WebsiteExists(Guid id)
        {
            return _context.Website.Any(e => e.WebsiteId == id);
        }
    }
}
