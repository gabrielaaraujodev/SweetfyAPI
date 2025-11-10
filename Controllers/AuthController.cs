using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SweetfyAPI.DTOs.AuthDTO;
using SweetfyAPI.DTOs.UserDTO;
using SweetfyAPI.Entities;
using SweetfyAPI.TokenService;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SweetfyAPI.Controllers
{
    /// <summary>
    /// Handles authentication, user registration, and token management.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/>.
        /// </summary>
        /// <param name="tokenService">Service for generating and validating tokens.</param>
        /// <param name="userManager">ASP.NET Identity service for managing users.</param>
        /// <param name="configuration">Application configuration to access JWT settings.</param>
        /// <param name="mapper">AutoMapper instance for DTO mapping.</param>
        public AuthController(
            ITokenService tokenService,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
        }

        /// <summary>
        /// Registers a new user and their associated bakery.
        /// </summary>
        /// <param name="model">The registration data (username, email, password, bakery name).</param>
        /// <returns>A success or error message.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResponseModel { Status = "Error", Message = "Invalid data." });

            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = "This email is already in use." });
            }

            var bakery = new Bakery { Name = model.BakeryName };

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.UserName,
                FullName = model.FullName,
                Bakery = bakery,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new ResponseModel { Status = "Error", Message = $"User creation failed: {errors}" });
            }

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        /// <summary>
        /// Logs in an existing user.
        /// </summary>
        /// <param name="model">The user's login credentials (email and password).</param>
        /// <returns>An access token and refresh token on success.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return Unauthorized(new ResponseModel { Status = "Error", Message = "Invalid data." });

            var user = await _userManager.FindByEmailAsync(model.Email!);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password!))
            {
                return Unauthorized(new ResponseModel { Status = "Error", Message = "Invalid email or password." });
            }

            var authClaims = await GetClaimsForUser(user);
            var token = _tokenService.GenerateAccessToken(authClaims, _configuration);
            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);
            await _userManager.UpdateAsync(user);

            return Ok(new TokenModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken
            });
        }

        /// <summary>
        /// Generates a new access token using a valid refresh token.
        /// </summary>
        /// <param name="tokenModel">A model containing the expired access token and the valid refresh token.</param>
        /// <returns>A new set of access and refresh tokens.</returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
        {
            if (tokenModel?.AccessToken is null || tokenModel.RefreshToken is null)
                return BadRequest(new ResponseModel { Status = "Error", Message = "Tokens are required." });

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken, _configuration);
            if (principal?.Identity?.Name is null)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = "Invalid access token." });
            }

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if (user == null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest(new ResponseModel { Status = "Error", Message = "Invalid or expired refresh token." });
            }

            var newAuthClaims = await GetClaimsForUser(user);
            var newAccessToken = _tokenService.GenerateAccessToken(newAuthClaims, _configuration);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return Ok(new TokenModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            });
        }

        /// <summary>
        /// Gets the profile information for the currently authenticated (logged-in) user.
        /// </summary>
        /// <returns>The user's public data (UserDto).</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserLogged()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == null)
                return Unauthorized(new ResponseModel { Status = "Error", Message = "Invalid token." });

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound(new ResponseModel { Status = "Error", Message = "User not found." });

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Private helper method to generate the list of claims for a user's JWT.
        /// </summary>
        /// <param name="user">The user entity.</param>
        /// <returns>A list of claims.</returns>
        private async Task<List<Claim>> GetClaimsForUser(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("BakeryId", user.BakeryId.ToString()!)
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }
            return authClaims;
        }
    }
}