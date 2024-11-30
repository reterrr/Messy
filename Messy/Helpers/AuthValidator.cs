using Messy.Contexts;
using Messy.Models;

namespace Messy.Helpers;

public class AuthValidator
{
    public User? ExtractValidUser(string username, string password)
    {
        // Find user by username        
        var user = MessyDbContext.Set<User>().FirstOrDefault(u => u.UserName == username);
        if (user != null &&
            PasswordHasher.Verify(password, user.Password)) return user;

        Message = "Invalid username or password";

        return null;
    }

    public bool UserExists(string matching)
    {
        return MessyDbContext.Set<User>().Any(u => u.UserName == matching);
    }

    public static AuthValidator Make()
    {
        return new AuthValidator();
    }

    private readonly MessyDbContext MessyDbContext = MessyDbContextFactory.CreateDbContext();
    
    public static string? Message;
}