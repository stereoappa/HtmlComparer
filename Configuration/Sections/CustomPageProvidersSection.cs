using HtmlComparer.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HtmlComparer.Configuration.Sections
{
    class CustomPageProvidersSection : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, XmlNode section)
        {
            List<ICustomPageProvider> myConfigObject = new List<ICustomPageProvider>();

            foreach (XmlNode childNode in section.ChildNodes)
            {
                myConfigObject.Add(Activator.CreateInstance(Type.GetType(childNode.Attributes["type"].Value)) as ICustomPageProvider);
            }
            return myConfigObject;
        }
    }
}

