using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ayeonbbsbackend.Utils
{
    public sealed class HtmlFillter
    {
        /// <summary>
        /// 替换html标签
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string ReplaceHtmlToBlank(string htmlStr)
        {
            if (string.IsNullOrEmpty(htmlStr))
                return "";
            return Regex.Replace(htmlStr, "<[^>]*>|&nbsp;", "");
        }

        /// <summary>
        /// 保留img标签，过滤HTML
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string ReplaceHtmlToBlankExCludetImg(string htmlStr)
        {
            if (string.IsNullOrEmpty(htmlStr))
                return "";
            //string stringPattern = @"</?(?(?=a|img|br|iframe|object|embed|param|tbody|td|tr|table|&nbsp;@)notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            string stringPattern = @"</?(?(?=figure|a|img@)notag|[a-zA-Z0-9]+)(?:\s[a-zA-Z0-9\-]+=?(?:(["",']?).*?\1?)?)*\s*/?>";
            return Regex.Replace(htmlStr, stringPattern, "");
        }
    }
}
