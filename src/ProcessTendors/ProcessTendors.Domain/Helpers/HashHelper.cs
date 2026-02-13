using System.Security.Cryptography;
using System.Text;

namespace ProcessTendors.Domain.Helpers
{
    public static class HashHelper
    {
        public static string ComputeHash(string input)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input.Trim());
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }
    }
}
