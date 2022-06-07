using System.IO;

namespace Stories.Services
{
    public static class Generator
    {
        public static string GenerateRandomString()
        {
            string path = Path.GetRandomFileName();
            path = path.Replace(".", ""); // Remove period.
            return path;
        }
    }
}