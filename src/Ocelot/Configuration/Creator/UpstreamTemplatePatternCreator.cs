using Ocelot.Configuration.File;
using Ocelot.Infrastructure.Tokenizer;
using Ocelot.Values;

namespace Ocelot.Configuration.Creator
{
    public class UpstreamTemplatePatternCreator : IUpstreamTemplatePatternCreator
    {
        private const string RegExForwardSlashOnly = "^/$";

        public UpstreamPathTemplate Create(IRoute route)
        {
            var upstreamTemplate = route.UpstreamPathTemplate;

            if (upstreamTemplate == "/")
            {
                return new UpstreamPathTemplate(RegExForwardSlashOnly, route.Priority, false, route.UpstreamPathTemplate);
            }

            var containsQueryString = upstreamTemplate.Contains('?');

            var regexTemplate = TemplateRegexBuilder.TemplateToRegex(upstreamTemplate, route.RouteIsCaseSensitive);

            return new UpstreamPathTemplate(
                regexTemplate,
                route.Priority,
                containsQueryString,
                route.UpstreamPathTemplate);
        }
    }
}
