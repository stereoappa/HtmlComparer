using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace HtmlComparer
{
    public class CompareFieldsSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<ComparedField> myConfigObject = new List<ComparedField>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var r = new ComparedField();

                r.Tag = childNode.Attributes["tag"] != null ? childNode.Attributes["tag"].Value : null;
                r.Name = childNode.Attributes["name"] != null ? childNode.Attributes["name"].Value : null;
                r.Attr = childNode.Attributes["attr"] != null ? childNode.Attributes["attr"].Value : null;
               
                myConfigObject.Add(r);

            }
            return myConfigObject;
        }
    }

    
}
