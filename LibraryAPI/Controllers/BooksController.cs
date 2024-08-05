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
    public class BooksController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public BooksController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet]
        
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            return await _context.Books.Where(b=>b.isDeleted==false).Include(p => p.Publisher).Include(b => b.AuthorBooks)!.ThenInclude(a => a.Author).Include(l => l.BookLanguages)!.ThenInclude(l => l.Language).Include(s => s.BookSubCategories)!.ThenInclude(su => su.SubCategory).ThenInclude(c => c!.Category).ToListAsync();

        }

        // Example of a custom action to search books by author ID
        [HttpGet("searchByAuthorID")]
       
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooksByAuthorId(int authorId)
        {
            List<Book> books = await _context.Books!
                .Where(b => b.isDeleted==false&&b.AuthorBooks!.Any(ab => ab.AuthorsId == authorId)).Include(p => p.Publisher).Include(b => b.AuthorBooks)!.ThenInclude(a => a.Author).Include(l => l.BookLanguages)!.ThenInclude(l => l.Language).Include(s => s.BookSubCategories)!.ThenInclude(su => su.SubCategory).ThenInclude(c => c!.Category)
                .ToListAsync();

            if (books == null)
            {
                return NotFound();
            }

            return books;
        }

        [HttpGet("searchByName")]
      
        public async Task<ActionResult<List<Book>>> SearchByName(string BookTitle)
        {
            List<Book> books = await _context.Books!
                .Where(b => b.Title.Contains(BookTitle)&& b.isDeleted==false).Include(p => p.Publisher).Include(b => b.AuthorBooks)!.ThenInclude(a => a.Author).Include(l => l.BookLanguages)!.ThenInclude(l => l.Language).Include(s => s.BookSubCategories)!.ThenInclude(su => su.SubCategory).ThenInclude(c => c!.Category)
                .ToListAsync();

            if (books == null)
            {
                return NotFound(); 
            }

            return books;
        }




        // GET: api/Books/5
        [HttpGet("{id}")]
        
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books!
                .Include(p => p.Publisher)
                .Include(b => b.AuthorBooks)!.ThenInclude(a => a.Author)
                .Include(l => l.BookLanguages)!.ThenInclude(l => l.Language)
                .Include(s => s.BookSubCategories)!.ThenInclude(su => su.SubCategory).ThenInclude(c => c!.Category)
                .FirstOrDefaultAsync(b => b.ID == id);
            if (book.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.ID)
            {
                return BadRequest();
            }
            if (book.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            if (book.PublishingYear > DateTime.Today.Year)
            {
                return BadRequest("Invalid publishsing year");
            }
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
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

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            AuthorBook authorBook;
            BookLanguage bookLanguage;
            BookSubCategory bookSubCategory;

            
          if (_context.Books == null)
          {
              return Problem("Entity set 'ApplicationContext.Books'  is null.");
          }
            if (book.PublishingYear > DateTime.Today.Year)
            {
                return BadRequest("Invalid publishsing year");
            }
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            if (book.AuthorIds != null)
            {
                foreach(long authorID in book.AuthorIds)
                {
                    authorBook = new AuthorBook();
                    authorBook.AuthorsId = authorID;
                    authorBook.BooksID = book.ID;
                    _context.AuthorBook.Add(authorBook);
                }
                _context.SaveChanges();
            }
            if (book.LanguagesCodes != null)
            {
                foreach (string languageCode in book.LanguagesCodes)
                {
                    bookLanguage = new BookLanguage();
                    bookLanguage.BooksID = book.ID;
                    bookLanguage.LanguagesCode = languageCode;
                    _context.BookLanguage.Add(bookLanguage);
                }
                _context.SaveChanges();
            }
            if (book.SubCategoriesIDs != null)
            {
                foreach (short SubCategoryID in book.SubCategoriesIDs)
                {
                    bookSubCategory = new BookSubCategory();
                    bookSubCategory.BooksID = book.ID;
                    bookSubCategory.SubCategoryId = SubCategoryID;
                    _context.BookSubCategory.Add(bookSubCategory);
                }
                _context.SaveChanges();
            }
            
            await _context.SaveChangesAsync();
            //    for (int i = 0; i < book.CopyCount; i++)
            //{
            //    BookCopy bookCopy = new BookCopy();
            //    bookCopy.IsBorrowed = false;
            //    bookCopy.BookID = book.ID;
            //    _context.BookCopies.Add(bookCopy);
            //}

            return CreatedAtAction("GetBook", new { id = book.ID }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (_context.Books == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            if (book.isDeleted)
            {
                return BadRequest("Book is deleted");
            }
            book.isDeleted = true;
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return (_context.Books?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
