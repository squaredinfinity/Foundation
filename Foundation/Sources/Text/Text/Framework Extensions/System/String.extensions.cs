using SquaredInfinity.Text;
using SquaredInfinity.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class StringExtensions
    {
        public static string InjectZeroWidthSpaces(this string str, bool splitCamelCase)
        {
            var sb = new StringBuilder();

            var insertZWSPBeforeNextNonLetterOrDigitCharacter = true;

            for(int i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (Char.IsLetter(c))
                {
                    if (splitCamelCase && Char.IsUpper(c))
                    {
                        if(i < str.Length - 1 && Char.IsLower(str[i + 1]))
                        {
                            // insert ZWSP before this upper case character, because it is followed by lower case character
                            sb.Append(UnicodeCharacters.Specials.LayoutControls.ZeroWidthSpace);
                        }
                    }

                    sb.Append(c);
                    insertZWSPBeforeNextNonLetterOrDigitCharacter = true;
                    continue;
                }

                if (insertZWSPBeforeNextNonLetterOrDigitCharacter)
                    sb.Append(UnicodeCharacters.Specials.LayoutControls.ZeroWidthSpace);

                sb.Append(c);

                if(i < str.Length - 1)
                    sb.Append(UnicodeCharacters.Specials.LayoutControls.ZeroWidthSpace);

                insertZWSPBeforeNextNonLetterOrDigitCharacter = false;
            }

            return sb.ToString();
        }
    }
}
