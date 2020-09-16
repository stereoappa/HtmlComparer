using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace HtmlComparer.Configuration.Sections
{
    public class PagesSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<Page> myConfigObject = new List<Page>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                foreach (XmlAttribute attrib in childNode.Attributes)
                {
                    myConfigObject.Add(new Page(attrib.Value));
                }
            }
            return myConfigObject;
        }
    }
}