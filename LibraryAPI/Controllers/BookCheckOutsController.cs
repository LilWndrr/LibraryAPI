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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookCheckOutsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;


        public BookCheckOutsController(ApplicationContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        
        }

        // GET: api/BookCheckOuts
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<BookCheckOut>>> GetBookCheckOuts()
        {
          if (_context.BookCheckOuts == null)
          {
              return NotFound();
          }
            return await _context.BookCheckOuts.ToListAsync();
        }

        // GET: api/BookCheckOuts/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<BookCheckOut>> GetBookCheckOut(long id)
        {
          if (_context.BookCheckOuts == null)
          {
              return NotFound();
          }
            var bookCheckOut = await _context.BookCheckOuts.FindAsync(id);

            if (bookCheckOut == null)
            {
                return NotFound();
            }

            return bookCheckOut;
        }

        // PUT: api/BookCheckOuts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutBookCheckOut(long id, BookCheckOut bookCheckOut)
        {
            if (id != bookCheckOut.ID)
            {
                return BadRequest();
            }
            BookCopy bookCopy = _context.BookCopies.FirstOrDefault(bc => bc.Id == bookCheckOut.BookCopyID);

            bookCopy.IsBorrowed = false;
            if (bookCheckOut.IsBookHarmed)
            {
                bookCopy.IsHarmed = true;
                var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("HarmedBook"));
                PunishmentInvoice punishment = new PunishmentInvoice();
                punishment.MemberId = bookCheckOut.MemberId;
                punishment.PunishmentReasonId = punishmentReason.Id;
                punishment.IsClosed = false;
                punishment.BookCheckoutId = bookCheckOut.ID;
                _context.PunishmentInvoices.Add(punishment);

            }

            
            if (bookCheckOut.ActualDateOfReturn > bookCheckOut.ReturnDate)
            {
                var dateDifference = bookCheckOut.ActualDateOfReturn.Subtract(bookCheckOut.ReturnDate);
                if (dateDifference.Days > 365)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn1year"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 180)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn6month"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 90)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn3month"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 60)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn2month"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 30)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn30days"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 15)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn15days"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 10)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn10days"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }
                else if (dateDifference.Days > 5)
                {
                    var punishmentReason = _context.PunishmentReasons.FirstOrDefault(p => p.Name.Contains("BookLateReturn5days"));
                    PunishmentInvoice punishment = new PunishmentInvoice();
                    punishment.MemberId = bookCheckOut.MemberId;
                    punishment.PunishmentReasonId = punishmentReason.Id;
                    punishment.IsClosed = false;
                    punishment.BookCheckoutId = bookCheckOut.ID;
                    _context.PunishmentInvoices.Add(punishment);
                }

            }

            string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;
            bookCheckOut.RecieverEmployeeId = appUser.Id;
            _context.Entry(bookCheckOut).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookCheckOutExists(id))
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

        // POST: api/BookCheckOuts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<BookCheckOut>> PostBookCheckOut(BookCheckOut bookCheckOut)
        {
          if (_context.BookCheckOuts == null)
          {
              return Problem("Entity set 'ApplicationContext.BookCheckOuts'  is null.");
          }
            string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;
            bookCheckOut.EmployeeId = appUser.Id;
            _context.BookCheckOuts.Add(bookCheckOut);
            await _context.SaveChangesAsync();
                
                    BookCopy bookCopy = _context.BookCopies.FirstOrDefaultAsync(bc => bc.Id == bookCheckOut.BookCopyID).Result;
                    if (bookCopy.IsBorrowed)
                    {
                        return BadRequest("Book is already borrowed");
                    }

                    if (bookCopy.IsHarmed)
                    {
                        return Problem("Book is harmed");
                    }
                    

                    bookCopy.IsBorrowed = true;
                 
                    _context.BookCopies.Update(bookCopy);
                
            
          
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBookCheckOut", new { id = bookCheckOut.ID }, bookCheckOut);
        }

        // DELETE: api/BookCheckOuts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteBookCheckOut(long id)
        {
            if (_context.BookCheckOuts == null)
            {
                return NotFound();
            }
            var bookCheckOut = await _context.BookCheckOuts.FindAsync(id);
            if (bookCheckOut == null)
            {
                return NotFound();
            }

            _context.BookCheckOuts.Remove(bookCheckOut);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookCheckOutExists(long id)
        {
            return (_context.BookCheckOuts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
