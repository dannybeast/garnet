using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace IcbcodeCMS.Areas.CMS.Utilities
{
    public class Transliterator
    {
        public static string Translite(string original, char delimiter)
        {
            Dictionary<string, string> words = new Dictionary<string, string>();

            words.Add("а", "a");
            words.Add("б", "b");
            words.Add("в", "v");
            words.Add("г", "g");
            words.Add("д", "d");
            words.Add("е", "e");
            words.Add("ё", "yo");
            words.Add("ж", "zh");
            words.Add("з", "z");
            words.Add("и", "i");
            words.Add("й", "j");
            words.Add("к", "k");
            words.Add("л", "l");
            words.Add("м", "m");
            words.Add("н", "n");
            words.Add("о", "o");
            words.Add("п", "p");
            words.Add("р", "r");
            words.Add("с", "s");
            words.Add("т", "t");
            words.Add("у", "u");
            words.Add("ф", "f");
            words.Add("х", "h");
            words.Add("ц", "c");
            words.Add("ч", "ch");
            words.Add("ш", "sh");
            words.Add("щ", "sch");
            words.Add("ъ", "j");
            words.Add("ы", "i");
            words.Add("ь", "j");
            words.Add("э", "e");
            words.Add("ю", "yu");
            words.Add("я", "ya");

            StringBuilder prepare = new StringBuilder();

            foreach (char chr in original)
            {
                prepare.Append(Char.IsLetterOrDigit(chr) ? chr : ' ');
            }

            original = prepare.ToString().ToLowerInvariant().Trim();

            foreach (KeyValuePair<string, string> pair in words)
            {
                original = original.Replace(pair.Key, pair.Value);
            }

            original = Regex.Replace(original, @"\s+", delimiter.ToString());

            return original;
        }

        public static string Translite(string original)
        {
            return Translite(original, '-');
        }
    }
}