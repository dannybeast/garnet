using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace IcbcodeCMS.Areas.CMS.Utilities
{
    public static class SignatureHelper
    {
        public static string GetSignature(string secretKey, IDictionary<string, string> formData)
        {
            var keys = formData
            .Where(x => x.Key != "signature")
            .Select(x => new { x.Key, Value = $"{x.Key}={GetBase64Val(x.Value)}" })
            .OrderBy(x => x.Key)
            .ToArray();

            var values = string.Join("&", keys.Select(x => x.Value));

            var signature = SHA1(secretKey + SHA1(secretKey + values));

            return signature;
        }


        private static string GetBase64Val(object plainText)
        {
            var value = plainText == null ? string.Empty : plainText.ToString();

            var plainTextBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(plainTextBytes);
        }

        private static string SHA1(string input)
        {
            using (var sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (var b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString().ToLowerInvariant();
            }
        }
    }
}