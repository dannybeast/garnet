using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace IcbcodeCMS.Areas.CMS.Models.Objects
{
    /// <summary> 
    /// Instances of this class are used to geneate alpha-numeric strings. 
    /// </summary> 
    public sealed class AlphaNumericStringGenerator
    {
        /// <summary> 
        /// The synchronization lock. 
        /// </summary> 
        private object _lock = new object();

        /// <summary> 
        /// The cryptographically-strong random number generator. 
        /// </summary> 
        private RNGCryptoServiceProvider _crypto = new RNGCryptoServiceProvider();

        /// <summary> 
        /// Construct a new instance of this class. 
        /// </summary> 
        public AlphaNumericStringGenerator()
        {
            //Nothing to do here. 
        }

        /// <summary> 
        /// Return a string of the provided length comprised of only uppercase alpha-numeric characters each of which are 
        /// selected randomly. 
        /// </summary> 
        /// <param name="ofLength">The length of the string which will be returned.</param> 
        /// <returns>Return a string of the provided length comprised of only uppercase alpha-numeric characters each of which are 
        /// selected randomly.</returns> 
        public string GetRandomUppercaseAlphaNumericValue(int ofLength)
        {
            lock (_lock)
            {
                var builder = new StringBuilder();

                for (int i = 1; i <= ofLength; i++)
                {
                    builder.Append(GetRandomUppercaseAphanumericCharacter());
                }

                return builder.ToString();
            }
        }

        /// <summary> 
        /// Return a randomly-generated uppercase alpha-numeric character (A-Z or 0-9). 
        /// </summary> 
        /// <returns>Return a randomly-generated uppercase alpha-numeric character (A-Z or 0-9).</returns> 
        private char GetRandomUppercaseAphanumericCharacter()
        {
            var possibleAlphaNumericValues =
             new char[]{'A','B','C','D','E','F','G','H','I','J','K','L',
       'M','N','O','P','Q','R','S','T','U','V','W','X','Y',
       'Z','0','1','2','3','4','5','6','7','8','9'};

            return possibleAlphaNumericValues[GetRandomInteger(0, possibleAlphaNumericValues.Length - 1)];
        }

        /// <summary> 
        /// Return a random integer between a lower bound and an upper bound. 
        /// </summary> 
        /// <param name="lowerBound">The lower-bound of the random integer that will be returned.</param> 
        /// <param name="upperBound">The upper-bound of the random integer that will be returned.</param> 
        /// <returns> Return a random integer between a lower bound and an upper bound.</returns> 
        private int GetRandomInteger(int lowerBound, int upperBound)
        {
            uint scale = uint.MaxValue;

            // we never want the value to exceed the maximum for a uint, 
            // so loop this until something less than max is found. 
            while (scale == uint.MaxValue)
            {
                byte[] fourBytes = new byte[4];
                _crypto.GetBytes(fourBytes); // Get four random bytes. 
                scale = BitConverter.ToUInt32(fourBytes, 0); // Convert that into an uint. 
            }

            var scaledPercentageOfMax = (scale / (double)uint.MaxValue); // get a value which is the percentage value where scale lies between a uint's min (0) and max value. 
            var range = upperBound - lowerBound;
            var scaledRange = range * scaledPercentageOfMax; // scale the range based on the percentage value 
            return (int)(lowerBound + scaledRange);
        }
    }

    public class IcbcodeUtility
    {
        public static string GetPromocode()
        {
            var generator = new AlphaNumericStringGenerator();

            return generator.GetRandomUppercaseAlphaNumericValue(7);
        }

        public static IDictionary<string, string> ToDictionary(NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }
            return dict;
        }

        public static void SendEmail(string from_email, string to_email, string title, string message)
        {
            var Settings = IcbcodeVariable.All(IcbcodeUtility.CurrentDomain);

            try
            {
                using (SmtpClient smtp = new SmtpClient(Settings["smtp-host"], Int32.Parse(Settings["smtp-port"])) { DeliveryMethod = SmtpDeliveryMethod.Network, Credentials = new NetworkCredential(Settings["smtp-login"], Settings["smtp-parolj"]), EnableSsl = Boolean.Parse(Settings["smtp-ispoljzovatj-ssl"]) })
                {
                    MailMessage msg = new MailMessage() { IsBodyHtml = true };

                    msg.From = string.IsNullOrEmpty(from_email) ? new MailAddress(Settings["smtp-login"], Settings["email-otpravka-priem"]) : new MailAddress(from_email, from_email);
                    msg.ReplyToList.Add(string.IsNullOrEmpty(from_email) ? new MailAddress(Settings["email-otpravka-priem"], Settings["email-otpravka-priem"]) : new MailAddress(from_email, from_email));

                    msg.Subject = title;

                    try
                    {
                        msg.To.Add(string.IsNullOrEmpty(to_email) ? new MailAddress(Settings["email-otpravka-priem"], Settings["email-otpravka-priem"]) : new MailAddress(to_email, to_email));

                        msg.Body = message;

                        smtp.Send(msg);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        public static long? DigitsOnly(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            StringBuilder r_Digits = new StringBuilder();

            foreach (char chr in input)
                if (Char.IsDigit(chr))
                    r_Digits.Append(chr);

            return Int64.Parse(r_Digits.ToString());
        }

        public static bool ContainsInQueryString(string name)
        {
            bool contains = false;

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                try
                {
                    contains = HttpContext.Current.Request.QueryString.AllKeys.Contains(name);
                }
                catch { }
            }

            return contains;
        }

        //public static dynamic GetNumericRange(long content_root, string field_name)
        //{
        //    dynamic item;

        //    using (UtilityRepositories utility = new UtilityRepositories())
        //    {
        //        item = utility.GetNumericRange(content_root, field_name);
        //    }

        //    return item;
        //}

        //public static List<dynamic> GetRange(long content_root, string field_name)
        //{
        //    List<dynamic> items;

        //    using (UtilityRepositories utility = new UtilityRepositories())
        //    {
        //        items = utility.GetRange(content_root, field_name);
        //    }

        //    return items;
        //}

        public static string FormatPhoneNumber(string format, string phone)
        {
            StringBuilder result = new StringBuilder(); int position = 0;

            foreach (char item in format)
            {
                if (item != '.')
                {
                    result.Append(item == '#' ? phone[position] : item);
                }

                if (item == '.' || item == '*' || item == '#') { position++; }
            }

            return result.ToString();
        }

        public static string NormalizePhoneNumbers(string phones)
        {
            List<string> n_phones = new List<string>();

            foreach (string phone in phones.Split(','))
            {
                n_phones.Add(NormalizePhoneNumber(phone));
            }

            return JoinInString(",", n_phones.ToArray());
        }

        public static string NormalizePhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return null;

            StringBuilder r_NormalizePhone = new StringBuilder();

            foreach (char chr in phone)
                if (Char.IsDigit(chr))
                    r_NormalizePhone.Append(chr);

            string _phone = (r_NormalizePhone.Length == 0 || r_NormalizePhone.ToString().StartsWith("8") == false) ? r_NormalizePhone.ToString() : string.Format("7{0}", r_NormalizePhone.ToString().Substring(1, r_NormalizePhone.Length - 1));


            return _phone.Length > 11 ? _phone.Substring(0, 11) : _phone;
        }

        public static string GeneratePasscode(int length)
        {
            string generated_password = CryptedRandomString() + CryptedRandomString();

            return generated_password.Substring(0, length);
        }

        private static string CryptedRandomString()
        {
            int rand = 0;

            byte[] randomNumber = new byte[5];

            System.Security.Cryptography.RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider();

            Gen.GetBytes(randomNumber);

            rand = Math.Abs(BitConverter.ToInt32(randomNumber, 0));

            return rand.ToString();
        }

        public static string BuildFullUrl(string local)
        {
            string full_url = null;

            HttpContext current = HttpContext.Current;

            if (current != null)
            {
                full_url = current.Request.Url.Host + "/" + local;
            }

            return full_url;
        }

        public static string FileSizeToString(long size)
        {
            string result = string.Empty;

            if (size < 1024)
            {
                result = string.Format("{0} байт", size);
            }
            else if (size >= 1024 && size < 1048576)
            {
                result = string.Format("{0} Кбайт", size / 1024);
            }
            else if (size >= 1048576)
            {
                result = string.Format("{0} Mбайт", Math.Round((decimal)size / (decimal)1048576, 2));
            }

            return result;
        }

        public static string PastInQueryString(params KeyValuePair<string, object>[] ks_vs)
        {
            return PastInQueryString(null, ks_vs);
        }

        public static string PastInQueryString(string localpath, params KeyValuePair<string, object>[] ks_vs)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();

            foreach (var item in ks_vs)
            {
                query.Add(item.Key, item.Value.ToString());
            }

            string url = string.Empty;

            HttpContext current = HttpContext.Current;

            if (current != null)
            {
                url = string.Format("{0}?", localpath == null ? current.Request.Url.LocalPath : localpath);

                foreach (string n in current.Request.QueryString)
                {
                    if (n == null) { continue; }

                    url = url + string.Format("{0}={1}&", n, query.ContainsKey(n) ? query[n] : current.Request.QueryString[n]);

                    if (query.ContainsKey(n))
                    {
                        query.Remove(n);
                    }
                }

                foreach (KeyValuePair<string, string> k_v in query)
                {
                    url = url + string.Format("{0}={1}&", k_v.Key, k_v.Value);
                }
            }

            return url.Trim(new char[] { '&' });
        }

        private static IcbcodeContent _current_user;

        public static IcbcodeContent User
        {
            get
            {
                HttpContext context = HttpContext.Current;

                if (context != null && context.User.Identity.IsAuthenticated)
                {
                    if (_current_user == null)
                    {
                        _current_user = IcbcodeContent.Get(new string[] { "partneri" }, null, null, 1, 1, "email = @email", new { email = context.User.Identity.Name }).FirstOrDefault();
                    }
                }
                else
                {
                    _current_user = null;
                }

                return _current_user;
            }
        }

        public static string CurrentDomain
        {
            get
            {
                HttpContext context = HttpContext.Current;

                return context == null ? null : context.Request.Url.Host;
            }
        }

        public static string GetPlainText(string html)
        {
            return GetPlainText(html, html == null ? 0 : html.Length);
        }

        public static string GetPlainText(string html, int length, string end_text = "")
        {
            string plain_text = System.Text.RegularExpressions.Regex.Replace(html ?? string.Empty, "<.*?>", string.Empty);

            plain_text = plain_text.Replace("&nbsp;", string.Empty);

            return plain_text.Substring(0, length > plain_text.Length ? plain_text.Length : length) + (length < plain_text.Length ? end_text : string.Empty);
        }

        public static T[] QueryString<T>(string name, T[] default_value)
        {
            T[] value = default_value; List<T> items = new List<T>();

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                try
                {
                    foreach (var item in HttpContext.Current.Request.QueryString[name].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        items.Add((T)Convert.ChangeType(item, typeof(T), CultureInfo.InvariantCulture));
                    }
                }
                catch { }
            }

            return items.ToArray();
        }

        public static T QueryString<T>(string name)
        {
            return QueryString<T>(name, default(T));
        }

        public static T QueryString<T>(string name, T default_value)
        {
            T value = default_value;

            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                string v = HttpContext.Current.Request.QueryString[name];

                try
                {
                    value = (T)Convert.ChangeType(v == "true,false" ? "true" : v, typeof(T), CultureInfo.InvariantCulture);
                }
                catch { }
            }

            return value;
        }

        public static T[] StringToArray<T>(string input, T[] default_value)
        {
            T[] value = default_value;

            if (input != null)
            {
                try
                {
                    value = input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(a => (T)Convert.ChangeType(a, typeof(T), CultureInfo.InvariantCulture)).ToArray<T>();

                    //value = (T[])Convert.ChangeType(new List<string>(input.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)), typeof(T[]), CultureInfo.InvariantCulture);
                }
                catch { }
            }

            return value;
        }

        public static string NormalizeUrl(string url)
        {
            return string.IsNullOrWhiteSpace(url) ? null : (url.StartsWith("http://", true, CultureInfo.InvariantCulture) || url.StartsWith("https://", true, CultureInfo.InvariantCulture) ? url : string.Format("http://{0}", url));
        }

        public static string JoinInString(string separator, params object[] items)
        {
            string result = string.Empty;

            foreach (var item in items ?? new object[0])
            {
                if (item != null)
                {
                    result += item.ToString() + (items.Last() == item ? string.Empty : separator);
                }
            }

            return result.Trim();
        }

        public static string JoinInString2(string separator, long[] items)
        {
            string result = string.Empty;

            foreach (var item in items ?? new long[0])
            {
                result += item.ToString() + (items.Last() == item ? string.Empty : separator);
            }

            return result.Trim();
        }

        /// <summary>
        /// aEndings Array Массив слов или окончаний для чисел (1, 4, 5), например ['яблоко', 'яблока', 'яблок']
        /// </summary>
        /// <param name="number"></param>
        /// <param name="endingArray"></param>
        /// <returns></returns>
        public static string GetNumEnding(long number, params string[] endingArray)
        {
            string ending;

            number = number % 100;

            if (number >= 11 && number <= 19)
            {
                ending = endingArray[2];
            }
            else
            {
                var i = number % 10;

                switch (i)
                {
                    case 1:
                        ending = endingArray[0];
                        break;

                    case 2:
                    case 3:
                    case 4:
                        ending = endingArray[1];
                        break;

                    default:
                        ending = endingArray[2];
                        break;
                }
            }

            return ending;
        }

        public static string HTMLSubstring(string html, int length)
        {
            if (html == null)
            {
                throw new ArgumentNullException("html");
            }

            List<string> unclosedTags = new List<string>();

            bool isQuoted = false;

            if (html.Length > length)
            {
                for (int i = 0; i < html.Length; i++)
                {
                    char currentCharacter = html[i];

                    char nextCharacter = ' ';

                    if (i < html.Length - 1)
                    {
                        nextCharacter = html[i + 1];
                    }

                    // Check if quotes are on.
                    if (!isQuoted)
                    {
                        if (currentCharacter == '<' && nextCharacter != ' ' && nextCharacter != '>')
                        {
                            if (nextCharacter != '/') // Open tag.
                            {
                                int startIndex = i + 1;

                                if (startIndex < html.Length)
                                {
                                    int finishIndex = html.IndexOf(">", startIndex);

                                    if (finishIndex > 0)
                                    {
                                        if (html[finishIndex - 1] != '/')
                                        {
                                            string tag = html.Substring(startIndex, finishIndex - startIndex);

                                            if (tag.Contains(" "))
                                            {
                                                int temporaryFinishIndex = html.IndexOf(" ", startIndex);

                                                tag = html.Substring(startIndex, temporaryFinishIndex - startIndex);
                                            }

                                            if (!tag.Equals("br", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                unclosedTags.Add(tag);
                                            }
                                        }

                                        int tagLength = finishIndex + 1 - i;

                                        length += tagLength;

                                        i = finishIndex;
                                    }
                                }
                            }
                            else if (nextCharacter == '/') // Close tag.
                            {
                                int startIndex = i + 2;

                                if (startIndex < html.Length)
                                {
                                    int finishIndex = html.IndexOf(">", startIndex);

                                    if (finishIndex > 0)
                                    {
                                        string tag = html.Substring(startIndex, finishIndex - startIndex);

                                        // FILO.
                                        int index = unclosedTags.LastIndexOf(tag);

                                        if (index >= 0)
                                        {
                                            unclosedTags.RemoveAt(index);

                                            int tagLength = finishIndex + 1 - i;

                                            length += tagLength;

                                            i = finishIndex;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (currentCharacter == '"')
                        {
                            isQuoted = false;
                        }
                    }

                    if (i >= length)
                    {
                        html = string.Format("{0}...", html.Substring(0, i));

                        unclosedTags.Reverse();

                        foreach (string unclosedTag in unclosedTags)
                        {
                            html += string.Format("</{0}>", unclosedTag);
                        }
                    }
                }
            }

            return html;
        }

        public static string HighlightKeyWords(string text, string keywords, char keywords_separator, string cssClass, bool fullMatch, bool substring = false, int text_length = 300)
        {
            if (text == String.Empty || keywords == String.Empty || cssClass == String.Empty)
                return text;

            var words = keywords.Split(new[] { keywords_separator }, StringSplitOptions.RemoveEmptyEntries);

            string result;

            if (substring)
            {
                text = GetPlainText(text);

                int index = text.IndexOf(fullMatch ? keywords : words[0], StringComparison.InvariantCultureIgnoreCase);

                int start_index = index > text_length + 1 ? index - text_length : 0;

                int end_index = text.Length > index + text_length + 1 ? index + text_length : text.Length;

                text = string.Format("<p>{0}</p>", text.Substring(start_index, end_index - start_index));
            }

            if (fullMatch)
            {
                result = words.Select(word => "\\b" + word.Trim() + "\\b")
                            .Aggregate(text, (current, pattern) =>
                                      Regex.Replace(current,
                                      pattern,
                                        string.Format("<span class=\"{0}\">{1}</span>",
                                        cssClass,
                                        "$0"),
                                        RegexOptions.IgnoreCase));
            }
            else
            {
                result = words.Select(word => word.Trim()).Aggregate(text,
                             (current, pattern) =>
                             Regex.Replace(current,
                                             pattern,
                                               string.Format("<span class=\"{0}\">{1}</span>",
                                               cssClass,
                                               "$0"),
                                               RegexOptions.IgnoreCase));

            }

            return result;
        }

        public static int CalcFontSize(int min_font_size, int max_font_size, int current_value, int max_value)
        {
            int max_increment_font_size = max_font_size - min_font_size;

            int current_increment_font_size = max_increment_font_size * current_value / max_value;

            return (max_value == 0 || max_value == 1) ? min_font_size : min_font_size + current_increment_font_size;
        }
    }
}