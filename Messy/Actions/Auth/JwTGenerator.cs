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
         // Add other claims as needed, e.g., roles, permissions
      };
      // Define security key and signing credentials
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigAccesser.Configuration.GetValue<string>("JwtSettings:SecretKey")));
      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

      // Create the token descriptor
      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(claims),
         Expires = DateTime.UtcNow.AddHours(1),  // Token expiration time
         SigningCredentials = credentials
      };

      // Create the token
      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
      
      // Return the generated JWT token
      return tokenHandler.WriteToken(token);;
   }
}