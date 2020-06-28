using System;
using System.Collections.Generic;

using StringSegment = System.ReadOnlyMemory<char>;

namespace Ocelot.Infrastructure.Tokenizer
{
    internal static class TemplateTokenizer
    {
        public enum TokenKind
        {
            /// <summary>Text token.</summary>
            Normal,

            /// <summary>Placeholder token located in path part but not at the last path segment.</summary>
            PathPlaceholder,

            /// <summary>Placeholder token located in the last path segment.</summary>
            PathCatchAllPlaceholder,

            /// <summary>Placeholder token located in the last path segment exactly at the beginning of the segment.</summary>
            PathOptionalSeparatorCatchAllPlaceholder,

            /// <summary>Placeholder token located in query part.</summary>
            QueryPlaceholder,
        }

        public readonly struct Token
        {
            public Token(StringSegment span, TokenKind kind)
            {
                Span = span;
                Kind = kind;
            }

            public StringSegment Span { get; }

            public TokenKind Kind { get; }
        }

        public static IEnumerable<Token> Tokenize(string template)
        {
            var templateSpan = template.AsMemory();

            var queryStartIndex = template.IndexOfQueryStart();
            var normalTokenStartIndex = 0;

            var placeholderStartIndex = template.IndexOfPlaceholderStart();
            var hasQuery = queryStartIndex.IsFound();

            while (placeholderStartIndex.IsFound())
            {
                // stop tokenization if no placeholder end symbol
                var placeholderEndIndex = template.IndexOfPlaceholderEnd(placeholderStartIndex);
                if (placeholderEndIndex.IsNotFound())
                {
                    break;
                }

                // yield normal token if not empty
                if (normalTokenStartIndex < placeholderStartIndex)
                {
                    yield return templateSpan.Token(normalTokenStartIndex, placeholderStartIndex - 1, TokenKind.Normal);
                }

                var nextPlaceholderStartIndex = template.IndexOfPlaceholderStart(placeholderEndIndex);

                // placeholder is a part of query segment
                if (hasQuery && placeholderStartIndex > queryStartIndex)
                {
                    yield return templateSpan.Token(placeholderStartIndex, placeholderEndIndex,
                        TokenKind.QueryPlaceholder);
                }

                // check if placeholder is last path placeholder.
                else
                {
                    var isLastPathPlaceholder = nextPlaceholderStartIndex.IsNotFound()
                        || (hasQuery && nextPlaceholderStartIndex > queryStartIndex);

                    bool isCatchAllPlaceholder = isLastPathPlaceholder
                        && template.IndexOfPathSegmentStart(placeholderEndIndex).IsNotFound();

                    if (isCatchAllPlaceholder)
                    {
                        var startsAtSeparator =
                            placeholderStartIndex > 0 && template.Prev(placeholderStartIndex) == '/';

                        var finalPathTokenKind = startsAtSeparator
                            ? TokenKind.PathOptionalSeparatorCatchAllPlaceholder
                            : TokenKind.PathCatchAllPlaceholder;

                        yield return templateSpan.Token(placeholderStartIndex, placeholderEndIndex, finalPathTokenKind);
                    }

                    // placeholder is a normal path placeholder
                    else
                    {
                        yield return templateSpan.Token(placeholderStartIndex, placeholderEndIndex,
                            TokenKind.PathPlaceholder);
                    }
                }

                placeholderStartIndex = nextPlaceholderStartIndex;
                normalTokenStartIndex = placeholderEndIndex.IsAtEnd(template.Length)
                    ? -1
                    : placeholderEndIndex + 1;
            }

            // yield rest of the template as a normal token (if not empty)
            if (normalTokenStartIndex.IsFound())
            {
                yield return templateSpan.Token(normalTokenStartIndex, template.Length - 1, TokenKind.Normal);
            }
        }
    }
}
