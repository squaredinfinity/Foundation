using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the specified string [is null or empty].
        /// </summary>
        /// <param name="str">input string</param>
        /// <returns>
        /// 	<c>true</c> if the specified string [is null or empty]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string str, bool treatWhitespaceOnlyStringAsEmpty = true)
        {
            if (treatWhitespaceOnlyStringAsEmpty)
                return string.IsNullOrWhiteSpace(str);
            else
                return string.IsNullOrEmpty(str);
        }

        public static string SplitCamelCase(this string str)
        {
            var sb = new StringBuilder();
            
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (Char.IsLetter(c))
                {
                    if (Char.IsUpper(c))
                    {
                        if (i < str.Length - 1 && Char.IsLower(str[i + 1]))
                        {
                            // insert SPACE before this upper case character, because it is followed by lower case character
                            sb.Append(" ");
                        }
                    }

                    sb.Append(c);
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string[] Split(this string str, char separator, StringSplitOptions options = StringSplitOptions.None)
        {
            return str.Split(new char[] { separator }, options);
        }

        /// <summary>
        /// Returns the substring using regex pattern.
        /// </summary>
        /// <param name="str">Input string</param>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <returns></returns>
        public static string Substring(this string str, string regexPattern)
        {
            var match = Regex.Match(str, regexPattern);

            return match.Value;
        }

        public static string Substring(this string str, string regexPattern, string capturingGroupName)
        {
            return str.Substring(regexPattern, capturingGroupName, true);
        }

        /// <summary>
        /// Returns the substring using regex pattern.
        /// The substring is a value captured by named group
        /// </summary>
        /// <param name="str">input string</param>
        /// <param name="regexPattern">The regex pattern.</param>
        /// <param name="capturingGroupName">Name of the capturing group.</param>
        /// <returns></returns>
        public static string Substring(this string str, string regexPattern, string capturingGroupName, bool throwOnFailure)
        {
            var match = Regex.Match(str, regexPattern);

            if (!match.Success)
            {
                if (throwOnFailure)
                {
                    Exception ex = new ArgumentException("no match found.");
                    throw ex;
                }
                else
                {
                    return string.Empty;
                }
            }

            var group = match.Groups[capturingGroupName];

            if (group == null || !group.Success)
            {
                if (throwOnFailure)
                {
                    Exception ex = new ArgumentException("capturing group does not exist.");
                    throw ex;
                }
                else
                {
                    return string.Empty;
                }
            }

            return group.Value;
        }

        public static bool IsMultiLine(this string str)
        {
            if (str.Contains(Environment.NewLine))
                return true;

            return false;
        }

        /// <summary>
        /// Returns length of the string with expanded tabs (if tabs were replaced with spaces ' ');
        /// For example, assuming default tabSize == 4,
        ///   "a\t"    = 4
        ///   "aa\t"   = 4
        ///   "aaa\t"  = 4
        ///   "aaaa\t" = 8
        ///   "\ta"    = 5
        ///   "\ta\t"  = 8
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabSize"></param>
        /// <returns></returns>
        public static int GetLengthWithExpandedTabs(this string str, ushort tabSize = 4)
        {
            if (str.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: false))
                return 0;

            if (tabSize == 1)
                return str.Length;

            var len = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\t')
                {
                    len += tabSize - (len % tabSize);
                }
                else
                {
                    len++;
                }
            }


            return len;
        }

        /// <summary>
        /// Exapnds tabs replacing them with spaces.
        /// For example, assuming default tabSize == 4,
        ///   "a\t"    => "a   "
        ///   "aa\t"   => "aa  "
        ///   "aaa\t"  => "aaa "
        ///   "aaaa\t" => "aaaa    "
        ///   "\ta"    => "    a"
        ///   "\ta\t"  => "    a   "
        /// </summary>
        /// <param name="str"></param>
        /// <param name="tabSize"></param>
        /// <returns></returns>
        public static string ExpandTabs(this string str, ushort tabSize = 4)
        {
            if (str.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: false))
                return str;

            if (tabSize == 1)
                return str.Replace('\t', ' ');

            var sb = new StringBuilder();

            var lines = str.GetLines(keepLineBreaks: true);

            for (int lx = 0; lx < lines.Length; lx++)
            {
                var line = lines[lx];
                var expandedIndex = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == '\t')
                    {
                        var expansionLen = tabSize - (expandedIndex % tabSize);
                        sb.Append(new string(' ', expansionLen));
                        expandedIndex += expansionLen;
                    }
                    else
                    {
                        sb.Append(line[i].ToString());
                        expandedIndex++;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Converts the string to a valid file name.
        /// Replaces all characters that are invalid in a filename with '-'.
        /// </summary>
        /// <param name="me">Me.</param>
        /// <returns></returns>
        public static string ToValidFileName(this string str, char replacementChar = '-')
        {
            var invalidFileNameChars = Path.GetInvalidFileNameChars();

            if (invalidFileNameChars.Contains(replacementChar))
            {
                throw new ArgumentException($"replacementChar [{replacementChar}] is an invalid filename char.");
            }

            StringBuilder sb = new StringBuilder();

            foreach (var ch in str)
            {
                if (invalidFileNameChars.Contains(ch))
                    sb.Append(replacementChar);
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <param name="str">string</param>
        /// <param name="keepLineBreaks">when true, line breaks will be included in returned lines</param>
        /// <returns></returns>
        public static string[] GetLines(this string str, bool keepLineBreaks = false)
        {
            if (keepLineBreaks)
            {
                List<string> result = new List<string>();

                var sb = new StringBuilder();

                for (int i = 0; i < str.Length; i++)
                {
                    if (i < str.Length - 1 && str[i] == '\r' && str[i + 1] == '\n')
                    {
                        sb.Append("\r\n");
                        i++;
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (str[i] == '\n')
                    {
                        sb.Append("\n");
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(str[i].ToString());
                    }
                }

                if (sb.Length > 0)
                {
                    result.Add(sb.ToString());
                }

                return result.ToArray();
            }
            else
            {
                var result = str.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                return result;
            }
        }

        public static string PrefixEachLine(this string str, string prefix)
        {
            var lines = str.GetLines();

            for (int i = 0; i < lines.Length; i++)
                lines[i] = prefix + lines[i];

            return string.Join(Environment.NewLine, lines);
        }

        public static bool Contains(this string str, string otherString, StringComparison comparisonType)
        {
            if (otherString == null)
                return false;

            return str.IndexOf(otherString, comparisonType) != -1;
        }

        public static bool Contains(this string str, string otherString, CultureInfo culture, CompareOptions compareOptions = CompareOptions.None)
        {
            if (otherString == null)
                return false;

            return culture.CompareInfo.IndexOf(str, otherString, compareOptions) != -1;
        }

        public static string TrimEachLine(this string str, bool removeEmptyLines = true)
        {
            var result = new StringBuilder(capacity: str.Length);

            var lines = str.GetLines();

            bool lastLineEmpty = false;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                var trimmedLine = line.Trim();

                if (trimmedLine.Length == 0)
                {
                    if (!removeEmptyLines)
                    {
                        lastLineEmpty = true;
                        result.AppendLine();
                    }

                    continue;
                }
                
                // add new line before if:
                //  - last line wasn't empty and this is not the first line to be added
                //  - last line was empty, and this is not the second line to be added
                if(!lastLineEmpty && result.Length > 0 || lastLineEmpty && i != 1)
                {
                    result.AppendLine();
                }

                result.Append(trimmedLine);

                lastLineEmpty = false;
            }

            if (lastLineEmpty)
                result.AppendLine();

            return result.ToString();
        }

    }
}
