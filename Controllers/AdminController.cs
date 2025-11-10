using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.AuthDTO;
using SweetfyAPI.Entities;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Manages administrative operations like role creation and user assignments.
    /// All endpoints require 'Admin' role authorization.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdminController"/>.
        /// </summary>
        /// <param name="userManager">ASP.NET Identity service for managing users.</param>
        /// <param name="roleManager">ASP.NET Identity service for managing roles.</param>
        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Creates a new role in the system.
        /// </summary>
        /// <param name="roleName">The name of the new role to create.</param>
        /// <returns>A success or error message.</returns>
        [HttpPost("roles/create")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest(new ResponseModel { Status = "Error", Message = "Role name cannot be empty." });

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
                return BadRequest(new ResponseModel { Status = "Error", Message = "Role already exists." });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                return Ok(new ResponseModel { Status = "Success", Message = $"Role {roleName} created successfully." });

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ResponseModel { Status = "Error", Message = errors });
        }

        /// <summary>
        /// Assigns an existing user to an existing role.
        /// </summary>
        /// <param name="email">The email of the user to assign.</param>
        /// <param name="roleName">The name of the role to assign to the user.</param>
        /// <returns>A success or error message.</returns>
        [HttpPost("roles/assign")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddUserToRole([FromQuery] string email, [FromQuery] string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ResponseModel { Status = "Error", Message = "User not found." });

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
                return NotFound(new ResponseModel { Status = "Error", Message = "Role not found." });

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
                return Ok(new ResponseModel { Status = "Success", Message = $"User {email} added to the {roleName} role." });

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ResponseModel { Status = "Error", Message = errors });
        }

        /// <summary>
        /// Revokes the refresh token for a specific user, effectively logging them out on their next refresh attempt.
        /// </summary>
        /// <param name="email">The email of the user whose token will be revoked.</param>
        /// <returns>No content if successful, or Not Found if the user doesn't exist.</returns>
        [HttpPost("users/revoke-token")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RevokeRefreshToken([FromQuery] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ResponseModel { Status = "Error", Message = "User not found." });

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}