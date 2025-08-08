using System.Security.Cryptography;
using System.Text;

namespace Techno_Home.Helpers
{
    public static class PasswordHelper
    {
        // Generates a random 16-byte salt encoded as a Base64 string.
        // Salt helps defend against precomputed hash attacks like rainbow tables.
        public static string GenerateSalt()
        {
            var rng = RandomNumberGenerator.Create();
            var saltBytes = new byte[16];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        // Hashes the password together with the provided salt using SHA256.
        // Returns a Base64-encoded string of the resulting hash.
        public static string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);     // Combine password and salt
            var hash = sha256.ComputeHash(combined);                      // Compute SHA256 hash
            return Convert.ToBase64String(hash);                                // Return as string
        }
    }
}