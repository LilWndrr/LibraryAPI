using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguagesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public LanguagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Language>>> GetLanguages()
        {
          if (_context.Languages == null)
          {
              return NotFound();
          }
            return await _context.Languages.Where(l=>l.isDeleted==false).ToListAsync();
        }

        // GET: api/Languages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Language>> GetLanguage(string id)
        {
          if (_context.Languages == null)
          {
              return NotFound();
          }
            var language = await _context.Languages.FindAsync(id);
            if (language.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            if (language == null)
            {
                return NotFound();
            }

            return language;
        }

        // PUT: api/Languages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutLanguage(string id, Language language)
        {
            if (id != language.Code)
            {
                return BadRequest();
            }
            if (language.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            _context.Entry(language).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguageExists(id))
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

        // POST: api/Languages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<Language>> PostLanguage(Language language)
        {
          if (_context.Languages == null)
          {
              return Problem("Entity set 'ApplicationContext.Languages'  is null.");
          }
            _context.Languages.Add(language);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (LanguageExists(language.Code))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetLanguage", new { id = language.Code }, language);
        }

        // DELETE: api/Languages/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteLanguage(string id)
        {
            if (_context.Languages == null)
            {
                return NotFound();
            }
            var language = await _context.Languages.FindAsync(id);
            if (language == null)
            {
                return NotFound();
            }
            if (language.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            language.isDeleted = true;
            _context.Languages.Update(language);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguageExists(string id)
        {
            return (_context.Languages?.Any(e => e.Code == id)).GetValueOrDefault();
        }
    }
}
