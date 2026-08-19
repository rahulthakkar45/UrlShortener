using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Application.Helpers
{
    public static class ShortCodeGenerator
    {
        private const string _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Generate(int length = 6)
        {
            var random = new Random();
            return new string(Enumerable.Repeat(_chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
