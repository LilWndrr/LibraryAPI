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
    public class PunishmentReasonsController : ControllerBase
    {
        private readonly ApplicationContext _context;

        public PunishmentReasonsController(ApplicationContext context)
        {
            _context = context;
        }

        // GET: api/PunishmentReasons
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<PunishmentReason>>> GetPunishmentReasons()
        {
          if (_context.PunishmentReasons == null)
          {
              return NotFound();
          }
            return await _context.PunishmentReasons.ToListAsync();
        }

       

       

        // POST: api/PunishmentReasons
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<PunishmentReason>> PostPunishmentReason()
        {
          if (_context.PunishmentReasons == null)
          {
              return Problem("Entity set 'ApplicationContext.PunishmentReasons'  is null.");
          }

            
                PunishmentReason punishmentReason = new PunishmentReason();
                punishmentReason.Name = "HarmedBook";
                punishmentReason.Description = "Book is harmed";
                punishmentReason.Price = 400;
                _context.PunishmentReasons.Add(punishmentReason);
                PunishmentReason punishmentReason2 = new PunishmentReason();
                punishmentReason2.Name = "BookLateReturn1year";
                punishmentReason2.Description = "";
                punishmentReason2.Price = 600;
                _context.PunishmentReasons.Add(punishmentReason2);
                PunishmentReason punishmentReason3 = new PunishmentReason();
                punishmentReason3.Name = "BookLateReturn6month";
                punishmentReason3.Description = "";
                punishmentReason3.Price = 300;
                _context.PunishmentReasons.Add(punishmentReason3);
                PunishmentReason punishmentReason4 = new PunishmentReason();
                punishmentReason4.Name = "BookLateReturn3month";
                punishmentReason4.Description = "";
                punishmentReason4.Price = 200;
                _context.PunishmentReasons.Add(punishmentReason4);
                PunishmentReason punishmentReason5 = new PunishmentReason();
                punishmentReason5.Name = "BookLateReturn2month";
                punishmentReason5.Description = "";
                punishmentReason5.Price = 100;
                _context.PunishmentReasons.Add(punishmentReason5);
                PunishmentReason punishmentReason6 = new PunishmentReason();
                punishmentReason6.Name = "BookLateReturn30days";
                punishmentReason6.Description = "";
                punishmentReason6.Price = 90;
                _context.PunishmentReasons.Add(punishmentReason6);
                PunishmentReason punishmentReason7 = new PunishmentReason();
                punishmentReason7.Name = "BookLateReturn15days";
                punishmentReason7.Description = "";
                punishmentReason7.Price = 50;
                _context.PunishmentReasons.Add(punishmentReason7);
                PunishmentReason punishmentReason8 = new PunishmentReason();
                punishmentReason8.Name = "BookLateReturn10days";
                punishmentReason8.Description = "";
                punishmentReason8.Price = 30;
                _context.PunishmentReasons.Add(punishmentReason8);
                PunishmentReason punishmentReason9 = new PunishmentReason();
                punishmentReason9.Name = "BookLateReturn5days";
                punishmentReason9.Description = "";
                punishmentReason9.Price = 15;
                _context.PunishmentReasons.Add(punishmentReason9);
                await _context.SaveChangesAsync();
            
                
               
            return Ok();
        }
            
          
        

      

        private bool PunishmentReasonExists(short id)
        {
            return (_context.PunishmentReasons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
