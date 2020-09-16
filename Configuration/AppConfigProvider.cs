using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace HtmlComparer.Configuration
{
    public static class AppConfigProvider
    {
        private const string SourcesSection = "sources";
        private const string PagesSection = "pages";
        private const string CompareTagsSection = "compareFields";

        public static IEnumerable<Source> GetSource()
        {
            return ConfigurationManager.GetSection(SourcesSection) as List<Source>;
        }

        public static IEnumerable<Page> GetPages()
        {
            return (ConfigurationManager.GetSection(PagesSection) as List<Page>)
                ?.Distinct();
        }

        public static List<TagMetadata> GetCompareTags()
        {
            return ConfigurationManager.GetSection(CompareTagsSection) as List<TagMetadata>;

        }
    }
}
