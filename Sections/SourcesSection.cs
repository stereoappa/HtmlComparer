using MetaComparer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MetaComparer
{
    public class SourcesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<Source> myConfigObject = new List<Source>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var r = new Source();

                r.BaseUrl = childNode.Attributes["baseUrl"] != null ? childNode.Attributes["baseUrl"].Value : null;
                r.ReturnUrlIsLowerCase = childNode.Attributes["return-url-lower-case"] != null ?
                                         childNode.Attributes["return-url-lower-case"].Value == "true" ? true : false
                                         : false;

                CompareRole cr;
                Enum.TryParse<CompareRole>(childNode.Attributes["compare-role"].Value, out cr);
                r.CompareRole = cr;
                myConfigObject.Add(r);

            }
            return myConfigObject;
        }
    }

}
