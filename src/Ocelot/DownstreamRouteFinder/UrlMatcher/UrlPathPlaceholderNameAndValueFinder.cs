using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Ocelot.Infrastructure.Tokenizer;
using Ocelot.Responses;

namespace Ocelot.DownstreamRouteFinder.UrlMatcher
{
    public class UrlPathPlaceholderNameAndValueFinder : IPlaceholderNameAndValueFinder
    {
        public Response<List<PlaceholderNameAndValue>> Find(string path, string query, string pathTemplate)
        {
            var regex = TemplateRegexBuilder.TemplateToRegex(pathTemplate, false);
            var containsQueryString = pathTemplate.Contains('?');
            var input = containsQueryString ? path + query : path;

            var match = new Regex(regex).Match(input);

            var values = match.Groups
                .Cast<Group>()
                .Where(g => g.Name != "0")
                .SelectMany(g =>
                    g.Captures
                        .Select(c => c.Value)
                        .DefaultIfEmpty("")
                        .Select(n => new PlaceholderNameAndValue($"{{{g.Name}}}", n)))
                .ToList();

            return new OkResponse<List<PlaceholderNameAndValue>>(values);
        }
    }
}
