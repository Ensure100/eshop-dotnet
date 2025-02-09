using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

[Route("api/auth")]
[ApiController]
public class AuthController : controllerBase
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

    }
  }
}