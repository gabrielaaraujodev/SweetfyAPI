using SweetfyAPI.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SweetfyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public int GetMyBakeryId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("We were unable to access the HttpContext.");
            }

            var bakeryIdClaim = httpContext.User.FindFirstValue("BakeryId");

            if (string.IsNullOrEmpty(bakeryIdClaim) || !int.TryParse(bakeryIdClaim, out var bakeryId))
            {

                throw new UnauthorizedAccessException("Claim 'BakeryId' not found or invalid in the token.");
            }

            return bakeryId;
        }

        public async Task<ApplicationUser?> GetMeAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("We were unable to access the HttpContext.");
            }

            var email = httpContext.User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                throw new UnauthorizedAccessException("Claim 'Email' not found in token.");
            }

            return await _userManager.FindByEmailAsync(email);
        }
    }
}