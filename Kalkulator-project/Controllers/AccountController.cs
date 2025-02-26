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
        
        [HttpPost("toggleOnSale/{productId}")]
        public async Task<ActionResult> ToggleOnSale(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found." });
            }

            product.OnSale = !product.OnSale;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Product onSale status updated.", ProductId = product.Id, NewStatus = product.OnSale });
        }
        
        [HttpGet("getProduct")]
        public IActionResult GetProductsWithSpecifications()
        {
            var productsWithSpec = _context.Products
                .Join(_context.Specifications, 
                    product => product.SpecificationId, 
                    spec => spec.Id, 
                    (product, spec) => new ProductWithSpecAndImageDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        OnSale = product.OnSale,
                        SellerId = product.SellerId,
                        SpecificationId = product.SpecificationId,
                        Specification = new SpecificationDto
                        {
                            BrandName = spec.BrandName,
                            Model = spec.Model,
                            FuelType = spec.FuelType,
                            ProductionDate = spec.ProductionDate,
                            Mileage = spec.Mileage
                        },
                        Image = product.Image  
                    })
                .ToList();

            return Ok(productsWithSpec);
        }

        
        [HttpPost("addProduct")]
        public async Task<ActionResult> AddProduct([FromBody] ProductWithSpecDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var specification = new Specification
                {
                    BrandName = model.Specification.BrandName,
                    Model = model.Specification.Model,
                    FuelType = model.Specification.FuelType,
                    ProductionDate = model.Specification.ProductionDate, 
                    Mileage = model.Specification.Mileage
                };

                _context.Specifications.Add(specification);
                await _context.SaveChangesAsync();

                var product = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    OnSale = model.OnSale,
                    SellerId = model.SellerId,
                    SpecificationId = specification.Id, 
                    Image = ""
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync(); 

                return Ok(new { Message = "Product added successfully.", ProductId = product.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Conflict(new { Message = "An error occurred.", Error = ex.InnerException?.Message ?? ex.Message });
            }
        }
        
        [HttpPost("uploadImage/{productId}")]
        public async Task<ActionResult> UploadImage(int productId, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "No file uploaded." });
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound(new { Message = "Product not found." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "public", "images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            product.Image = $"/public/images/{fileName}";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Image uploaded successfully.", ImagePath = product.Image });
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

            var (token, expiry, role) = _tokenProvider.Create(user);
            
            return Ok(new
            {
                Token = token,
                Expiry = expiry,
                Role = role,
                UserId = user.Id
            });
        }
        
        
    }
}
