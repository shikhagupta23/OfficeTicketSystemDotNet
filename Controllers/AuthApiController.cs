using Entities.Common;
using Entities.DTO;
using Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OfficeTicketSystemBackend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthApiController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _config;

		public AuthApiController(UserManager<ApplicationUser> userManager,
		RoleManager<IdentityRole> roleManager,
		IConfiguration config)
		{
			_roleManager = roleManager;
			_userManager = userManager;
			_config = config;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(RegisterDto dto)
		{
			var response = new ApiResponse();

			try
			{
				if (!await _roleManager.RoleExistsAsync(dto.Role))
				{
					response.IsSuccess = false;
					response.Message = "Invalid role";
					return BadRequest(response);
				}

				var user = new ApplicationUser
				{
					UserName = dto.Email,
					Email = dto.Email,
					FirstName = dto.FirstName,
					LastName = dto.LastName,
					PhoneNumber = dto.PhoneNumber
				};

				var result = await _userManager.CreateAsync(user, dto.Password);

				if (!result.Succeeded)
				{
					response.IsSuccess = false;
					response.Message = string.Join(", ",
						result.Errors.Select(e => e.Description));

					return BadRequest(response);
				}

				await _userManager.AddToRoleAsync(user, dto.Role);

				response.IsSuccess = true;
				response.Message = "User registered successfully";
				response.Id = user.Id;

				return Ok(response);
			}
			catch (Exception ex)
			{

				response.IsSuccess = false;
				response.Message = "Something went wrong while registering the user";

				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginDto dto)
		{
			var response = new ApiResponse<LoginResponseDto>();

			try
			{
				var user = await _userManager.Users
					.FirstOrDefaultAsync(u =>
						u.PhoneNumber == dto.UserId ||
						u.Email == dto.UserId);

				if (user == null)
				{
					response.IsSuccess = false;
					response.Message = "Invalid credentials";
					return Unauthorized(response);
				}

				var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);

				if (!isValid)
				{
					response.IsSuccess = false;
					response.Message = "Invalid credentials";
					return Unauthorized(response);
				}

				var token = await GenerateJwtToken(user);

				response.IsSuccess = true;
				response.Message = "Login successful";
				response.Data = new LoginResponseDto
				{
					Token = token,
					UserId = user.Id,
					Email = user.Email
				};

				return Ok(response);
			}
			catch (Exception ex)
			{

				response.IsSuccess = false;
				response.Message = "Something went wrong while logging in";

				return StatusCode(StatusCodes.Status500InternalServerError, response);
			}
		}


		private async Task<string> GenerateJwtToken(ApplicationUser user)
		{
			var roles = await _userManager.GetRolesAsync(user);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email),
				new Claim("LoggedUserUserId", user.Id),
				new Claim(ClaimTypes.NameIdentifier, $"{user.FirstName + " " + user.LastName}")
			};

			foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _config["Jwt:Issuer"],
				audience: _config["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}


	}
}
