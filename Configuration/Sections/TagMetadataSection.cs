using HtmlComparer.Model;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace HtmlComparer.Configuration.Sections
{
    public class TagMetadataSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<TagMetadata> myConfigObject = new List<TagMetadata>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                var r = new TagMetadata();

                r.TagName = childNode.Attributes["tag"] != null ? childNode.Attributes["tag"].Value : null;
                r.NameAttrValue = childNode.Attributes["name"] != null ? childNode.Attributes["name"].Value : null;
                r.ComparedAttrName = childNode.Attributes["attr"] != null ? childNode.Attributes["attr"].Value : null;
               
                myConfigObject.Add(r);

            }
            return myConfigObject;
        }
    }

    
}
