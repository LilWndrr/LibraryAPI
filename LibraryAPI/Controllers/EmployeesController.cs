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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics.Metrics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;


        public EmployeesController(ApplicationContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // GET: api/Employees
        [HttpGet]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
          if (_context.Employees == null)
          {
              return NotFound();
          }
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<Employee>> GetEmployee(string id)
        {
            string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;


            if (!User.IsInRole("Admin")) {
                
            if (appUser.Id != id)
            {
                return Unauthorized("It is not your business");
            }
            }
          if (_context.Employees == null)
          {
                return NotFound();
          }
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employees/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> PutEmployee(string id, Employee employee,string? currentPassword= null)
        {

            if (!User.IsInRole("Admin"))
            {
                string userName = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                AppUser appUser1 = _userManager.FindByNameAsync(userName).Result;
                if (appUser1.Id != id)
                {
                    return Unauthorized();
                }
            }
            AppUser appUser = _userManager.FindByIdAsync(id).Result;
            if (id != employee.Id)
            {
                return BadRequest();
            }

            appUser.Address = employee.AppUser!.Address;
            appUser.BirthDate = employee.AppUser.BirthDate;
            appUser.Email = employee.AppUser.Email;
            appUser.FamilyName = employee.AppUser.FamilyName;
            appUser.Gender = employee.AppUser.Gender;
            appUser.MiddleName = employee.AppUser.MiddleName;
            appUser.FamilyName = employee.AppUser.FamilyName;
            appUser.Status = employee.AppUser.Status;
            appUser.Password = employee.AppUser.Password;

         

            _userManager.UpdateAsync(appUser).Wait();
            if (currentPassword != null)
            {
                _userManager.ChangePasswordAsync(appUser, currentPassword, appUser.Password).Wait();
            }
            employee.AppUser = null;

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
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

        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            //Claim claim;
          if (_context.Employees == null)
          {
              return Problem("Entity set 'ApplicationContext.Employees'  is null.");
          }

            _userManager.CreateAsync(employee.AppUser!, employee.AppUser!.Password).Wait();
            _userManager.AddToRoleAsync(employee.AppUser, "Employee").Wait();
            //foreach(string category1 in category)
            //{
            //    claim = new("Category", category1);
            //    _userManager.AddClaimAsync(employee.AppUser, claim).Wait();
            //}
         
            employee.Id = employee.AppUser!.Id;
            employee.AppUser = null;
            _context.Employees.Add(employee);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EmployeeExists(employee.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEmployee", new { id = employee.Id }, employee);
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            if (_context.Employees == null)
            {
                return NotFound();
            }
            AppUser appUser = _userManager.FindByIdAsync(id).Result;
            if (appUser == null)
            {
                return NotFound();
            }
            appUser.IsDeleted = true;
            await _userManager.UpdateAsync(appUser);

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeIsDeletedStatus")]
        public async Task<ActionResult> AccountChangeIsDeletedStatus(string employeeID)
        {
            AppUser appUser = _userManager.FindByIdAsync(employeeID).Result;

            appUser.IsDeleted = false;
            await _userManager.UpdateAsync(appUser);

            return Ok();
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest("Username and password are required.");
            }

            var appUser = await _userManager.FindByNameAsync(userName);
            if (appUser.IsDeleted)
            {
                return Problem("Your account has been deleted. If you wish to reactivate it, please contact our support team.");
            }

            if (appUser == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(appUser, password, false);

            if (signInResult.Succeeded)
            {
                var token = await CreateJwtToken(appUser);
                return Ok(new { Token = new JwtSecurityTokenHandler().WriteToken(token) });
            } 

            return Unauthorized("Invalid username or password.");
        }

        [Authorize]
        [HttpGet("Logout")]
        public async Task<ActionResult> Logout()
        {
          

            await _signInManager.SignOutAsync();
            return Ok("Logout successful.");
        }

        [HttpPost("ForgetPassword")]
        public ActionResult<string> ForgetPassword(string userName)
        {
            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

                string token= _userManager.GeneratePasswordResetTokenAsync(appUser).Result;
            return token;
        }
        
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(string userName, string token, string newPassword)
        {

            AppUser appUser = _userManager.FindByNameAsync(userName).Result;

            var result =await _userManager.ResetPasswordAsync(appUser, token, newPassword);
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
               
                new Claim("uid", user.Id)
               
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1); // Example: token expires in 1 hour

            var jwtSecurityToken = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private bool EmployeeExists(string id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
