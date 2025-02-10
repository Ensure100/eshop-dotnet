using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

app.MapPost("/register", async ([FromBody] RegisterModel model, UserManager<IdentityUser> userManager) =>
{
    if (await userManager.FindByEmailAsync(model.Email) != null)
        return Results.BadRequest("User already exists.");

    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    var result = await userManager.CreateAsync(user, model.Password);

    return result.Succeeded ? Results.Ok("User registered successfully") : Results.BadRequest(result.Errors);
});

app.MapPost("/login", async ([FromBody] LoginModel model, UserManager<IdentityUser> userManager, IConfiguration config) =>
{
    var user = await userManager.FindByEmailAsync(model.Email);
    if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
        return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Email) }),
        Expires = DateTime.UtcNow.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
});

app.Run();

record RegisterModel(string Email, string Password);
record LoginModel(string Email, string Password);
