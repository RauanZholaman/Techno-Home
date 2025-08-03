using Microsoft.AspNetCore.Mvc;
using Techno_Home.Data;
using Techno_Home.Models;
using System.Security.Cryptography;
using System.Text;
using Techno_Home.Helpers;

namespace Techno_Home.Controllers;

public class AccountController : Controller
{
    private readonly StoreDbContext _context;

    public AccountController(StoreDbContext context)
    {
        _context = context;
    }
    
    // GET: /Account/Login
    public IActionResult Login() => View();
    
    //POST: /Account/Login
    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserName == username);
        if (user == null || !VerifyPassword(password, user.Salt, user.HashedPw))
        {
            ViewBag.Error = "Username or password is incorrect";
            return View();
        }
        
        HttpContext.Session.SetInt32("UserId", user.UserId);
        HttpContext.Session.SetString("UserName", user.UserName);
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
        if (_context.Users.Any(u => u.UserName == user.UserName))
        {
            ViewBag.Error = "Username is already taken";
            return View();
        }
        
        CreatePasswordHash(password, out var hash, out var salt);
        user.HashedPw = hash;
        user.Salt = salt;
        user.IsAdmin = false;
        
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return RedirectToAction("Login");
    }
    
    //GET: /Account/Logout
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
    
    //Password helpers
    private static void CreatePasswordHash(string password, out string hashedPW, out string salt)
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        salt = Convert.ToBase64String(saltBytes);
        
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + salt));
        hashedPW = Convert.ToBase64String(hash);
    }

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

        var user = _context.Users.FirstOrDefault(u => u.UserName == model.UserName);

        if (user == null)
        {
            ModelState.AddModelError("", "User not found.");
            return View(model);
        }

        var newSalt = PasswordHelper.GenerateSalt();
        var newHashed = PasswordHelper.HashPassword(model.NewPassword, newSalt);

        user.Salt = newSalt;
        user.HashedPw = newHashed;

        _context.SaveChanges();

        ViewBag.Message = "Password reset successful. You may log in.";
        return RedirectToAction("Login");
    }
    
    
}