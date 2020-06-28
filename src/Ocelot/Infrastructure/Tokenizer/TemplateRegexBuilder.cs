using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocelot.Infrastructure.Tokenizer
{
    internal static class TemplateRegexBuilder
    {
        public static string TemplateToRegex(string template, bool caseSensitive)
        {
            var hasQueryPlaceholder = false;

            var result = new StringBuilder();
            result.AppendRegexStart(caseSensitive);
            foreach (var token in TemplateTokenizer.Tokenize(template))
            {
                switch (token.Kind)
                {
                    case TemplateTokenizer.TokenKind.Normal:
                        result.Append(Regex.Escape(token.Span.ToString()));
                        break;
                    case TemplateTokenizer.TokenKind.PathPlaceholder:
                        result.AppendRegexPlaceholder(token, "[^/?]+");
                        break;
                    case TemplateTokenizer.TokenKind.PathCatchAllPlaceholder:
                        result.AppendRegexPlaceholder(token, ".*");
                        break;
                    case TemplateTokenizer.TokenKind.PathOptionalSeparatorCatchAllPlaceholder:
                        // trim final separator
                        result.Length--;

                        // require match to start with / or ?
                        result
                            .Append("(|/?")
                            .AppendRegexPlaceholder(token, ".*")
                            .Append(")");
                        break;
                    case TemplateTokenizer.TokenKind.QueryPlaceholder:
                        result.AppendRegexPlaceholder(token, "[^&]+");
                        hasQueryPlaceholder = true;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(token.Kind));
                }
            }

            if (result[^1] == '/')
            {
                // replace final separator
                result.Length--;
                result.Append("/?");
            }

            // append match all for rest of the upstream query
            if (hasQueryPlaceholder)
            {
                result.Append(".*");
            }

            result.AppendRegexEnd();
            return result.ToString();
        }
    }
}
