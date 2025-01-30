using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Messy.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace Messy.Actions.Auth;

public static class JwTGenerator
{
    public static string Generate(Models.User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(ConfigAccesser.Configuration.GetValue<string>("JwtSettings:SecretKey")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials
        };


        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);


        return tokenHandler.WriteToken(token);
    }
}