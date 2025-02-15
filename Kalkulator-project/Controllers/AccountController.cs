using System.Security.Claims;
using Kalkulator_project.Entities;
using Kalkulator_project.Infrastructure;
using Kalkulator_project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kalkulator_project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenProvider _tokenProvider;
        
        public AccountController(AppDbContext appDbContext, TokenProvider tokenProvider)
        {
            _context = appDbContext;
            _tokenProvider = tokenProvider;
        }
        

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var account = new UserAccount
            {
                Username = model.Username,
                Password = model.Password
            };

            try
            {
                _context.UserAccounts.Add(account);
                _context.SaveChanges();
                return Ok(new { Message = $"User {account.Username} registered successfully." });
            }
            catch (DbUpdateException)
            {
                return Conflict("Username must be unique.");
            }
        }
        
        [HttpGet("calculate")]
        [Authorize]
        public async Task<ActionResult> GetServices()
        {
            var services = await _context.Services
                .Select(s => new { s.Id, s.Name, s.Price })
                .ToListAsync();

            return Ok(services);
        }

        [HttpPost("calculate")]
        [Authorize]
        public async Task<ActionResult> PostCalculation([FromBody] ServicesViewModel model)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == model.ServiceId);
            if (service == null)
            {
                return NotFound("Service not found.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not found.");
            }

            var userServiceCalculation = new UserServices
            {
                UserId = userId,
                ServiceId = model.ServiceId,
                Area = model.Area,
                TotalCost = model.TotalCost, 
                CalculationDate = DateTime.UtcNow
            };

            _context.UserServices.Add(userServiceCalculation);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Service = service.Name,
                TotalCost = userServiceCalculation.TotalCost,
                Area = userServiceCalculation.Area,
                CalculationDate = userServiceCalculation.CalculationDate
            });
        }
    

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model, TokenProvider provider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _context.UserAccounts.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            string token = _tokenProvider.Create(user);
            
            return token != null ? Ok(new { Token = token }) : Unauthorized();
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout successful.");
        }
        
    }
}
