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
    public class PunishmentInvoicesController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PunishmentInvoicesController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/PunishmentInvoices
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<PunishmentInvoice>>> GetPunishmentInvoices()
        {
          if (_context.PunishmentInvoices == null)
          {
              return NotFound();
          }
            return await _context.PunishmentInvoices.Include(p=>p.PunishmentReason).Include(p=>p.BookCheckOut).ToListAsync();
        }

        // GET: api/PunishmentInvoices/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<PunishmentInvoice>> GetPunishmentInvoice(long id)
        {
          if (_context.PunishmentInvoices == null)
          {
              return NotFound();
          }
            var punishmentInvoice = await _context.PunishmentInvoices.FindAsync(id);

            if (punishmentInvoice == null)
            {
                return NotFound();
            }

            return punishmentInvoice;
        }

        // PUT: api/PunishmentInvoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutPunishmentInvoice(long id, PunishmentInvoice punishmentInvoice)
        {
            if (id != punishmentInvoice.ID)
            {
                return BadRequest();
            }

            _context.Entry(punishmentInvoice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PunishmentInvoiceExists(id))
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

        // POST: api/PunishmentInvoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<PunishmentInvoice>> PostPunishmentInvoice(PunishmentInvoice punishmentInvoice)
        {
          if (_context.PunishmentInvoices == null)
          {
              return Problem("Entity set 'ApplicationContext.PunishmentInvoices'  is null.");
          }
            _context.PunishmentInvoices.Add(punishmentInvoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPunishmentInvoice", new { id = punishmentInvoice.ID }, punishmentInvoice);
        }

        

        private bool PunishmentInvoiceExists(long id)
        {
            return (_context.PunishmentInvoices?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
