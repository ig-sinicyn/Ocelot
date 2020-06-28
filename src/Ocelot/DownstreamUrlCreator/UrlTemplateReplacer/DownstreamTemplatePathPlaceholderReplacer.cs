using System.Collections.Generic;
using System.Text.RegularExpressions;

using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Responses;
using Ocelot.Values;

namespace Ocelot.DownstreamUrlCreator.UrlTemplateReplacer
{
    public class DownstreamTemplatePathPlaceholderReplacer : IDownstreamPathPlaceholderReplacer
    {
        // matches "{...}" tokens
        private readonly Regex _placeholderRegex = new Regex("\\{[^}]*\\}", RegexOptions.Compiled);

        public Response<DownstreamPath> Replace(
            string downstreamPathTemplate,
            List<PlaceholderNameAndValue> urlPathPlaceholderNameAndValues)
        {
            if (string.IsNullOrEmpty(downstreamPathTemplate))
            {
                return new OkResponse<DownstreamPath>(new DownstreamPath(""));
            }

            var firstTemplateValues = new Dictionary<string, string>(urlPathPlaceholderNameAndValues.Count);
            foreach (var nameAndValue in urlPathPlaceholderNameAndValues)
            {
                firstTemplateValues.TryAdd(nameAndValue.Name, nameAndValue.Value);
            }

            var downstreamPath = _placeholderRegex.Replace(downstreamPathTemplate, m => firstTemplateValues[m.Groups[0].Value]);

            return new OkResponse<DownstreamPath>(new DownstreamPath(downstreamPath));
        }
    }
}
