using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;

namespace HtmlComparer
{
    public static class ConfigProvider
    {
        private const string SourcesSection = "sources";
        private const string PagesSection = "pages";
        private const string CompareTagsSection = "compareFields";

        public static List<Source> GetSource()
        {
            return ConfigurationManager.GetSection(SourcesSection) as List<Source>;
        }

        public static List<Page> GetPages()
        {
            return ConfigurationManager.GetSection(PagesSection) as List<Page>;

        }

        public static List<ComparedTag> GetCompareTags()
        {
            return ConfigurationManager.GetSection(CompareTagsSection) as List<ComparedTag>;

        }
    }
}
