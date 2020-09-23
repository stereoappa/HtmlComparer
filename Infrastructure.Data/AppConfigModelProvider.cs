using HtmlComparer.Configuration.Sections;
using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace HtmlComparer.Infrastructure.Data
{
    public class AppConfigModelProvider
    {
        private const string ModulesSection = "modules";
        private const string SourcesSection = "sources";
        private const string PagesSection = "pages";
        private const string CustomPageProviders = "customPageProviders";

        public IEnumerable<Source> GetSource()
        {
            return ConfigurationManager.GetSection(SourcesSection) as IEnumerable<Source>;
        }

        public IEnumerable<Page> GetPages()
        {
            return (ConfigurationManager.GetSection(PagesSection) as IEnumerable<Page>)
                ?.Distinct();
        }
        public IEnumerable<ICustomPageProvider> GetCustomPageProviders()
        {
            return (ConfigurationManager.GetSection(CustomPageProviders) as IEnumerable<ICustomPageProvider>);
        }


        public IEnumerable<IPagesComparer> GetComparers()
        {
            var modules = ConfigurationManager.GetSection(ModulesSection) as ModulesSection;

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

        public IEnumerable<IPageChecker> GetCheckers()
        {
            var modules = ConfigurationManager.GetSection(ModulesSection) as ModulesSection;

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
