using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity
{
    /// <summary>
    /// Implementation of CF_HTML Clipboard Format
    /// https://msdn.microsoft.com/en-us/library/aa767917(v=vs.85).aspx
    /// </summary>
    public static class CF_HTML
    {
        public static string PrepareHtmlFragment(string html)
        {
            return PrepareHtmlFragment(html, title: null, sourceUrl: null);
        }

        public static string PrepareHtmlFragment(string html, string title)
        {
            return PrepareHtmlFragment(html, title, sourceUrl: null);
        }

        public static string PrepareHtmlFragment(string html, string title, string sourceUrl)
        {
            //  NOTE: CF_HTML specifies that everything should use UTF-8

            var uses_custom_selection = false;

            var sb = new StringBuilder();

            var header =
@"Version:1.0
StartHTML:<<CF_HTML1>>
EndHTML:<<CF_HTML2>>
StartFragment:<<CF_HTML3>>
EndFragment:<<CF_HTML4>>
";
            if (uses_custom_selection)
            {
                throw new NotImplementedException("Custom selection is not yet implemented.");
                header +=
@"StartSelection:<<CF_HTML5>>
EndSelection:<<CF_HTML6>>
";
            }

            if (sourceUrl != null)
            {
                header +=
@"SourceURL:<<CF_HTML7>>
";
            }

            sb.Append(header);
            sb.Replace("<<CF_HTML1>>", Encoding.UTF8.GetBytes(sb.ToString()).Length.ToString("D12"));

            var pre = $@"<!DOCTYPE html><HTML><HEAD><meta charset=""UTF-8""><TITLE>{title}</TITLE></HEAD><BODY><!--StartFragment-->";

            sb.Append(pre);
            sb.Replace("<<CF_HTML3>>", Encoding.UTF8.GetBytes(sb.ToString()).Length.ToString("D12"));

            sb.Append(html);
            sb.Replace("<<CF_HTML4>>", Encoding.UTF8.GetBytes(sb.ToString()).Length.ToString("D12"));

            var post = @"<!--EndFragment--></BODY></HTML>";

            sb.Append(post);
            sb.Replace("<<CF_HTML2>>", Encoding.UTF8.GetBytes(sb.ToString()).Length.ToString("D12"));

            return sb.ToString();
        }
    }
}
