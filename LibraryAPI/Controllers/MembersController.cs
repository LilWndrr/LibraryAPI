using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;


        public MembersController(ApplicationContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Members
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
            return await _context.Members.Include(m => m.BookCheckOuts).Include(m => m.AppUser).Include(m => m.PunishmentHistory)!.ThenInclude(m => m.PunishmentReason).ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Member,Employee,Admin")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

            if (!User.IsInRole("Admin")||!User.IsInRole("Employee"))
            {
                if (appUser.Id != id)
                {
                    return Unauthorized("It is not your business");
                }
            }

            if (_context.Members == null)
          {
              return NotFound();
          }
            var member = await _context.Members.FindAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            return member;
        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutMember(string id, Member member, string? CurrentPassword= null)
        {
            if (!User.IsInRole("Admin")||!User.IsInRole("Employee")) {
                string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                AppUser appUser1 = _userManager.FindByNameAsync(userName).Result;
                if (appUser1.Id != id)
                {
                    return Unauthorized();
                }
            }

            AppUser appUser = _userManager.FindByIdAsync(id).Result;
            if (id != member.Id)
            {
                return BadRequest();
            }
            appUser.Address = member.AppUser!.Address;
            appUser.BirthDate = member.AppUser.BirthDate;
            appUser.Email = member.AppUser.Email;
            appUser.FamilyName = member.AppUser.FamilyName;
            appUser.Gender = member.AppUser.Gender;
            appUser.MiddleName = member.AppUser.MiddleName;
            appUser.FamilyName = member.AppUser.FamilyName;
            appUser.Status = member.AppUser.Status;
            appUser.Password = member.AppUser.Password;

            _userManager.UpdateAsync(appUser).Wait();
            if (CurrentPassword != null)
            {
                _userManager.ChangePasswordAsync(appUser, CurrentPassword, appUser.Password).Wait();
            }
            member.AppUser = null;

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
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

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<Member>> PostMember(Member member)
        {
          if (_context.Members == null)
          {
              return Problem("Entity set 'ApplicationContext.Members'  is null.");
          }

            _userManager.CreateAsync(member.AppUser!, member.AppUser!.Password).Wait();
            _userManager.AddToRoleAsync(member.AppUser,"Member").Wait();
            member.Id = member.AppUser.Id;
            member.AppUser = null;
            _context.Members.Add(member); 
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MemberExists(member.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMember", new { id = member.Id }, member);
        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            if (_context.Members == null)
            {
                return NotFound();
            }
        
            
            AppUser appUser = _userManager.FindByIdAsync(id).Result;
            if (appUser == null)
            {
                return NotFound();
            }
            if (appUser.IsDeleted)
            {
                return Problem("Your account is deleted,if you want to activate it contact to our staff");
            }
            appUser.IsDeleted = true;
            await _userManager.UpdateAsync(appUser);
            return NoContent();
        }

        [HttpGet("GetBooksInBorrow")]
        public async Task<List<BookCopy>> GetBooksInBorrow(string memberID)
        {

            List<BookCopy> bookcopies=  await _context.BookCopies.Where(bc=>bc.BookCheckOut.MemberId==memberID&& bc.BookCheckOut.ActualDateOfReturn==DateTime.MinValue).Include(bc=>bc.Book).ToListAsync();

            return bookcopies;
        }

        [Authorize(Roles ="Employee,Admin")]
        [HttpPost("ChangeIsDeletedStatus")]
        public async Task<ActionResult> AccountChangeIsDeletedStatus(string MemberID)
        {
            AppUser appUser = _userManager.FindByIdAsync(MemberID).Result;

            appUser.IsDeleted = false;
            await _userManager.UpdateAsync(appUser);

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {

            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            //_signInManager.ExternalLoginSignInAsync()
            if (appUser != null)
            {
                if (appUser.IsDeleted)
                {
                    return Problem("Your account is deleted,if you want to activate it contact to our staff");
                }

                signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);
                if (signInResult.Succeeded)
                {
                    var token = await CreateJwtToken(appUser);
                    return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
            }
            return Unauthorized();

        }

        [Authorize]
        [HttpGet("Logout")]
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

            string token = _userManager.GeneratePasswordResetTokenAsync(appUser).Result;
            return token;
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(string userName, string token, string newPassword)
        {

            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

            var result = await _userManager.ResetPasswordAsync(appUser, token, newPassword);
            if (!result.Succeeded)
            {
                return BadRequest("Smth went wrong");
            }

            return Ok();
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("uid", user.Id)
    }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("c5GZ63fhP1VW7e0TZcAeLWlTNq8lKYWl"));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1); // Example: token expires in 1 hour

            var jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private bool MemberExists(string id)
        {
            return (_context.Members?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
