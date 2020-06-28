using System;
using System.Runtime.CompilerServices;

namespace Ocelot.Infrastructure.Tokenizer
{
    internal static class TemplateTokenizerHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TemplateTokenizer.Token Token(
            this ReadOnlyMemory<char> source,
            int startIndex,
            int endIndex,
            TemplateTokenizer.TokenKind tokenKind) =>
            new TemplateTokenizer.Token(
                source.Slice(startIndex, endIndex - startIndex + 1),
                tokenKind);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFound(this int index) => index >= 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotFound(this int index) => index < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAtEnd(this int index, int length) => index == length - 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static char Prev(this string source, int index) => source[index - 1];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfPathSegmentStart(this string source, int startIndex = 0) => source.IndexOf('/', startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfPlaceholderStart(this string source, int startIndex = 0) => source.IndexOf('{', startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfPlaceholderEnd(this string source, int startIndex = 0) => source.IndexOf('}', startIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOfQueryStart(this string source) => source.IndexOf('?');
    }
}
