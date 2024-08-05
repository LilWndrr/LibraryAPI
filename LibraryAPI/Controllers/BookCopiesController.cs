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
    [ApiController]
    public class BookCopiesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BookCopiesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/BookCopies
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<BookCopy>>> GetBookCopies()
        {
          if (_context.BookCopies == null)
          {
              return NotFound();
          }
            return await _context.BookCopies.Where(bc=>bc.isDeleted==false).Include(bc=>bc.Book).ToListAsync();
        }

        // GET: api/BookCopies/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<BookCopy>> GetBookCopy(int id)
        {
          if (_context.BookCopies == null)
          {
              return NotFound();
          }
          
            var bookCopy = await _context.BookCopies.FindAsync(id);
            if (bookCopy!.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            if (bookCopy == null)
            {
                return NotFound();
            }

            return bookCopy;
        }

        // PUT: api/BookCopies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutBookCopy(int id, BookCopy bookCopy)
        {
            if (id != bookCopy.Id)
            {
                return BadRequest();
            }

            if (bookCopy.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            _context.Entry(bookCopy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCopyExists(id))
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

        // POST: api/BookCopies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<BookCopy>> PostBookCopy(BookCopy bookCopy)
        {
            if (_context.BookCopies == null)
            {
                return Problem("Entity set 'ApplicationContext.BookCopies' is null.");
            }
            
            _context.BookCopies.Add(bookCopy);

            // Ensure to await the changes to the BookCopies before accessing the book
            await _context.SaveChangesAsync();

            var book = await _context.Books.FirstOrDefaultAsync(b => b.ID == bookCopy.BookID);

            if (book == null)
            {
                return NotFound("Book not found.");
            }

            // Increment CopyCount correctly
            book.CopyCount++;

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookCopy", new { id = bookCopy.Id }, bookCopy);
        }


        // DELETE: api/BookCopies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteBookCopy(int id)
        {
            if (_context.BookCopies == null)
            {
                return NotFound();
            }
            var bookCopy = await _context.BookCopies.FindAsync(id);
            if (bookCopy == null)
            {
                return NotFound();
            }
            if (bookCopy.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            bookCopy.isDeleted = true;
            _context.BookCopies.Update(bookCopy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookCopyExists(int id)
        {
            return (_context.BookCopies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
