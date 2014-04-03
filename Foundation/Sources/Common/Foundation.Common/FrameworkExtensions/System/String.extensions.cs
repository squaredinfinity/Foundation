using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class StringExtensions
    {
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
        public static int GetLengthWithExpandedTabs(this string str, int tabSize = 4)
        {
            if (str.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: false))
                return 0;

            if (tabSize == 1)
                return str.Length;

            if (tabSize <= 0)
                throw new ArgumentException("tabSize must be > 0");

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
        public static string ExpandTabs(this string str, int tabSize = 4)
        {
            if (str.IsNullOrEmpty(treatWhitespaceOnlyStringAsEmpty: false))
                return str;

            if (tabSize == 1)
                return str.Replace('\t', ' ');

            if (tabSize <= 0)
                throw new ArgumentException("tabSize must be > 0");

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
        /// Uses specified string as format template for string.Format()
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string FormatWith(this string str, params object[] args)
        {
            return string.Format(str, args);
        }

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
                throw new ArgumentException("replacementChar [{0}] is an invalid filename char.".FormatWith(replacementChar));
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

    }
}
