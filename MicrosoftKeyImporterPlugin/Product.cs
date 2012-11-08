using System;
using System.Collections.Generic;
using System.Xml;

namespace MicrosoftKeyImporterPlugin
{
    class Product
    {
        private XmlNode _node;
        public Product(XmlNode node)
        {
            _node = node;
        }

        public String Name { get { return _node.Attributes["Name"].Value.Replace("\n", string.Empty); } }

        public List<Key> Keys
        {

            get {
                return GetKeys();
            }
        }

        private List<Key> GetKeys()
        {
            var keys = new List<Key>();

            foreach (XmlNode key in _node.ChildNodes)
            {
                keys.Add(new Key
                             {
                                 Value = key.InnerText,
                                 Type = key.Attributes != null ? key.Attributes["Type"].Value : string.Empty,
                                 Description = key.FirstChild.NodeType == XmlNodeType.CDATA ? key.InnerText : string.Empty
                             });
            }

            return keys;
        }
    }
}