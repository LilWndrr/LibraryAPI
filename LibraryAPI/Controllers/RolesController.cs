using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // POST api/roles/createroles
        [HttpPost("createroles")]
        [Authorize(Roles = "Employee,Admin")]
        public async Task<IActionResult> CreateRoles()
        {
            // Create "Member" role
            var memberRoleExists = await _roleManager.RoleExistsAsync("Member");
            if (!memberRoleExists)
            {
                var memberRole = new IdentityRole("Member");
                var result = await _roleManager.CreateAsync(memberRole);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to create role 'Member'.");
                }
            }

            // Create "Employee" role
            var employeeRoleExists = await _roleManager.RoleExistsAsync("Employee");
            if (!employeeRoleExists)
            {
                var employeeRole = new IdentityRole("Employee");
                var result = await _roleManager.CreateAsync(employeeRole);
                if (!result.Succeeded)
                {
                    return BadRequest("Failed to create role 'Employee'.");
                }
            }

            return Ok("Roles created successfully.");
        }
    }
}
