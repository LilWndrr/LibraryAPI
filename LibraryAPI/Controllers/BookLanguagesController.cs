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

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin,Employee")]
    [ApiController]
    public class BookLanguagesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookLanguagesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookLanguages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookLanguage>>> GetBookLanguage()
        {
          if (_context.BookLanguage == null)
          {
              return NotFound();
          }
            return await _context.BookLanguage.ToListAsync();
        }



        // POST: api/BookLanguages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookLanguage>> PostBookLanguage(BookLanguage bookLanguage)
        {
          if (_context.BookLanguage == null)
          {
              return Problem("Entity set 'ApplicationContext.BookLanguage'  is null.");
          }
            _context.BookLanguage.Add(bookLanguage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (BookLanguageExists(bookLanguage.BooksID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetBookLanguage", new { id = bookLanguage.BooksID }, bookLanguage);
        }

        // DELETE: api/BookLanguages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookLanguage(int bookId,string LangId)
        {
            if (_context.BookLanguage == null)
            {
                return NotFound();
            }
            var bookLanguage = await _context.BookLanguage.FindAsync(bookId,LangId);
            if (bookLanguage == null)
            {
                return NotFound();
            }

            _context.BookLanguage.Remove(bookLanguage);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookLanguageExists(int id)
        {
            return (_context.BookLanguage?.Any(e => e.BooksID == id)).GetValueOrDefault();
        }
    }
}
