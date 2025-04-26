using FridgeProject.Data.Models;
using FridgeProject.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Security.Claims;

namespace FridgeProject.Controllers
{
    [Route("api/role")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManeger;
        public RolesController(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            _roleManeger = roleManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole([FromBody]dtoAssignRole dtoAssignRole)
        {
            var user=await _userManager.FindByEmailAsync(dtoAssignRole.UserEmail);
            if (user == null) {return NotFound("user not exist");}

            var role =await _roleManeger.RoleExistsAsync(dtoAssignRole.RoleName);
            if (!role) { return NotFound("role not exist"); }

            var result = await _userManager.AddToRoleAsync(user, dtoAssignRole.RoleName);

           
            return result.Succeeded ? Ok($"user{dtoAssignRole.UserEmail} added to {dtoAssignRole.RoleName} Role") 
                : BadRequest($"Fail assign user{dtoAssignRole.UserEmail} added to {dtoAssignRole.RoleName} Role");
        }
       
    }
}
