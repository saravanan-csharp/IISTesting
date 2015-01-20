using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISDBLibrary
{
    public static class Generator
    {
        public static string GenerateUserName()
        {
            Random r = new Random();
            string alphabet = "abcdefghijklmnopqrstuvwyxzeeeiouea";
            Func<char> randomLetter = () => alphabet[r.Next(alphabet.Length)];
            Func<int, string> makeName =
              (length) => new string(Enumerable.Range(0, length)
                 .Select(x => x == 0 ? char.ToUpper(randomLetter()) : randomLetter())
                 .ToArray());

            string first = makeName(r.Next(5) + 5);
           
            return first;
        }

        public static string GeneratePassword()
        {
            Random r = new Random();
            string alphabet = "abc}@!<.>fgIJKLmnopQRstuVh1234567890-+={";
            Func<char> randomLetter = () => alphabet[r.Next(alphabet.Length)];
            Func<int, string> makeName =
              (length) => new string(Enumerable.Range(0, length)
                 .Select(x => x == 0 ? char.ToUpper(randomLetter()) : randomLetter())
                 .ToArray());

            string first = makeName(r.Next(5) + 5);


            return first;
        }
    }
}
