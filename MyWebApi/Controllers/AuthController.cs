using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MyWebApi.Data;
using MyWebApi.Models;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
  private readonly AppDbContext _context;

  public AuthController(AppDbContext context)
  {
    _context = context;
  }

  [HttpPost("register")]
  public IActionResult Register(RegisterDto model)
  {
    if (_context.Users.Any(u => u.Email == model.Email))
      return BadRequest(new { message = "Email already exists!"});

    var user = new User
    {
      FirstName = model.FirstName,
      LastName = model.LastName,
      Email = model.Email,
      PasswordHash = HashPassword(model.Password),
      PhoneNumber = model.PhoneNumber,
      Role = "User",
    };
    _context.Users.Add(user);
    _context.SaveChanges();

    return Ok(new { message = "User registered successfully!"});
  }

  [HttpPost("login")]
  public IActionResult Login(LoginDto model)
  {
    var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
    if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
      return Unauthorized(new { message = "Invalid email or password!"});

    return Ok(new { message = "Login successful", role = user.Role });
  }

  private string HashPassword(string password)
  {
    if (string.IsNullOrEmpty(password))
    {
      throw new ArgumentNullException(nameof(password), "Password connt be null or empty.");
    }
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
  }

  private bool VerifyPassword(string password, string hash)
  {
    if (string.IsNullOrEmpty(password))
    {
        throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
    }
    
    if (string.IsNullOrEmpty(hash))
    {
        throw new ArgumentNullException(nameof(hash), "Hash cannot be null or empty.");
    }
    return HashPassword(password) == hash;
  }
}