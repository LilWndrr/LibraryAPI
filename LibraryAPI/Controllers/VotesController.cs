using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;

        public VotesController(ApplicationContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Votes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Vote>>> GetVotes()
        {
          if (_context.Votes == null)
          {
              return NotFound();
          }
            return await _context.Votes.ToListAsync();
        }

        
        

        // POST: api/Votes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles ="Member")]
        public async Task<ActionResult<Vote>> PostVote(Vote vote)
        {
              if (_context.Votes == null)
          {
              return Problem("Entity set 'ApplicationContext.Votes'  is null.");
          }
            string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;
            vote.MemberId =appUser.Id;
            var testVote =_context.Votes.FirstOrDefault(v=>v.MemberId==appUser.Id&&v.BookId==vote.BookId);
            if (testVote!=null)
            {
                return BadRequest("You can only vote once ");
            }
            var bookChekouts = _context.BookCheckOuts.FirstOrDefault(b => b.MemberId == vote.MemberId && b.BookCopy.BookID == vote.BookId);
            if (bookChekouts == null)
            {
                return BadRequest("You cannot vote for book you haven't read");
            }
            
            _context.Votes.Add(vote);
            var book =_context.Books.FirstOrDefault(b => b.ID == vote.BookId);
            book.VotesCount++;
            book.SumOfVotes += vote.VoteValue;
            book.Rating= (float) Math.Round((book.SumOfVotes/book.VotesCount),1);
            _context.Books.Update(book);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (VoteExists(vote.BookId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok();
        }

       

        private bool VoteExists(int id)
        {
            return (_context.Votes?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
