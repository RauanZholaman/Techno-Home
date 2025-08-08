using Microsoft.AspNetCore.Mvc;
using Techno_Home.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Techno_Home.Helpers;
using Techno_Home.Data;

namespace Techno_Home.Controllers;

public class AccountController : Controller
{
    private readonly StoreDbContext _context;

    public AccountController(StoreDbContext context)
    {
        _context = context;
    }
    
    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login() => View();
    
    [HttpPost]
    public IActionResult Login(string email, string password)
    {
        // Attempt to find a user with the given email
        var user = _context.Users.SingleOrDefault(u => u.Email == email);
        
        // If no user is found or password doesn't match, show error
        if (user == null || !VerifyPassword(password, user.Salt, user.HashedPw))
        {
            ViewBag.Error = "Email or password is incorrect";
            return View();
        }

        // Store user information in session after successful login
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("UserName", user.UserName);
        HttpContext.Session.SetString("Email", user.Email);
        HttpContext.Session.SetString("isAdmin", user.IsAdmin ? "true" : "false");

        return RedirectToAction("Index", "Home");
    }
    
    //GET: /Account/Register
    [HttpGet]
    public IActionResult Register() => View();
    
    //POST: /Account/Register
    [HttpPost]
    public IActionResult Register(User user, string password)
    {
        // Check if the email is already registered
        if (_context.Users.Any(u => u.Email == user.Email))
        {
            ViewBag.Error = "Email is already registered";
            return View();
        }
        
        // Generate password hash and salt before saving
        CreatePasswordHash(password, out var hash, out var salt);
        user.HashedPw = hash;
        user.Salt = salt;
        user.IsAdmin = false;   // This ensures new users are not admins by default
        
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return RedirectToAction("Login");
    }
    
    
    //GET: /Account/Logout
    public IActionResult Logout()
    {
        // Clear all session data and redirect to home
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    
    // Helper method to create password hash and salt
    private static void CreatePasswordHash(string password, out string hashedPW, out string salt)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);    // Generate random salt
        salt = Convert.ToBase64String(saltBytes);
        
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt)); // Hash using SHA256
        hashedPW = Convert.ToBase64String(hash);
    }

    // Helper method to verify if password matches stored hash
    private static bool VerifyPassword(string password, string salt, string storedHash)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        return Convert.ToBase64String(hash) == storedHash;
    }

    [HttpGet]
    public IActionResult ResetPassword()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        // Attempt to find user by email
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
     
        if (user == null)
        {
            ModelState.AddModelError("", "User not found.");
            return View(model);
        }

        // Generate new password hash and salt, and update the user
        var newSalt = PasswordHelper.GenerateSalt();
        var newHashed = PasswordHelper.HashPassword(model.NewPassword, newSalt);

        user.Salt = newSalt;
        user.HashedPw = newHashed;

        _context.SaveChanges();

        ViewBag.Message = "Password reset successful. You may log in.";
        return RedirectToAction("Login");
    }
}