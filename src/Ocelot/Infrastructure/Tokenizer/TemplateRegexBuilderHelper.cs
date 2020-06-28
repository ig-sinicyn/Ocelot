using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Ocelot.Infrastructure.Tokenizer
{
    internal static class TemplateRegexBuilderHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendRegexStart(this StringBuilder builder, bool caseSensitive) =>
            caseSensitive
                ? builder.Append("^(?n)") // named groups only
                : builder.Append("^(?in)"); // case-insensitive; named groups only

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendRegexEnd(this StringBuilder builder) =>
            builder.Append("$");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendRegexPlaceholder(this StringBuilder builder, TemplateTokenizer.Token placeholderToken, string regex) =>
            builder
                .Append('(')
                .AppendRegexOptionalGroupName(placeholderToken.GetPlaceholderName())
                .Append(regex)
                .Append(")");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static StringBuilder AppendRegexOptionalGroupName(this StringBuilder builder, ReadOnlyMemory<char> groupName) =>
            groupName.Length == 0
                ? builder
                : builder.Append("?'")
                    .Append(Regex.Escape(groupName.ToString()))
                    .Append("'");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char> GetPlaceholderName(this TemplateTokenizer.Token token)
        {
            if (token.Kind == TemplateTokenizer.TokenKind.Normal)
            {
                throw new ArgumentException(
                    $"Cannot get placeholder name for normal token {token.Span}",
                    nameof(token.Kind));
            }

            return token.Span.Slice(1, token.Span.Length - 2);
        }
    }
}
