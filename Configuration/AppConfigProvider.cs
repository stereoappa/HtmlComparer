using HtmlComparer.Configuration.Sections;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace HtmlComparer.Configuration
{
    public static class AppConfigProvider
    {
        private const string SourcesSection = "sources";
        private const string PagesSection = "pages";

        public static IEnumerable<Source> GetSource()
        {
            return ConfigurationManager.GetSection(SourcesSection) as List<Source>;
        }

        public static IEnumerable<Page> GetPages()
        {
            return (ConfigurationManager.GetSection(PagesSection) as List<Page>)
                ?.Distinct();
        }

        public static IEnumerable<IPagesComparer> GetComparers()
        {
            var modules = ConfigurationManager.GetSection("modules") as ModulesSection;

            var res = new List<IPagesComparer>();
            for (int i = 0; i < modules.Comparers.Count; i++)
            {
                var comparers = modules.Comparers[i];
                var constructorParams = ConfigurationManager.GetSection(comparers.ConstructorParamsSection);

                res.Add(constructorParams != null ?
                    Activator.CreateInstance(Type.GetType(comparers.Type), constructorParams) as IPagesComparer :
                    Activator.CreateInstance(Type.GetType(comparers.Type)) as IPagesComparer);
            }

            return res;
        }

        public static IEnumerable<IPageChecker> GetCheckers()
        {
            var modules = ConfigurationManager.GetSection("modules") as ModulesSection;

            var res = new List<IPageChecker>();
            for (int i = 0; i < modules.Checkers.Count; i++)
            {
                var conf = modules.Checkers[i];
                var constructorParams = ConfigurationManager.GetSection(conf.ConstructorParamsSection);

                res.Add(constructorParams != null ?
                    Activator.CreateInstance(Type.GetType(conf.Type), constructorParams) as IPageChecker :
                    Activator.CreateInstance(Type.GetType(conf.Type)) as IPageChecker);
            }

            return res;
        }
    }
}
