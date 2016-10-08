using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace LuckyBallsData.Util
{
    public class DBXmlNode
    {
        private XElement mNativeNode;

        public DBXmlNode(XElement node)
        {
            mNativeNode = node;
        }

        public DBXmlDocument OwnerDocument()
        {
            return new DBXmlDocument(mNativeNode.Document);
        }

        public DBXmlNode AddChild(string name)
        {
            XElement child = new XElement(name);
            mNativeNode.Add(child);

            return new DBXmlNode(child);
        }

        public string GetAttribute(string name)
        {
            XAttribute attri = mNativeNode.Attribute(name);
            if (attri != null)
                return attri.Value;

            return null;
        }

        public void SetAttribute(string name, string value)
        {
            XAttribute attri = mNativeNode.Attribute(name);
            if (attri != null)
                attri.Value = value;
            else
                mNativeNode.Add(new XAttribute(name, value));
        }

        public List<DBXmlNode> ChildNodes()
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (var sub in mNativeNode.Elements())
            {
                XElement element = sub as XElement;
                if (element != null)
                    childnodes.Add(new DBXmlNode(element));
            }

            return childnodes;
        }

        public List<DBXmlNode> ChildNodes(string nodeName)
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (XElement element in mNativeNode.Elements(nodeName))
            {
                childnodes.Add(new DBXmlNode(element));
            }

            return childnodes;
        }

        public DBXmlNode FirstChildNode(string nodeName)
        {
            XElement element = mNativeNode.Element(nodeName);
            if (element != null)
            {
                return new DBXmlNode(element);
            }

            return null;
        }

        public DBXmlNode FirstChildNode()
        {
            return new DBXmlNode(mNativeNode.FirstNode as XElement);
        }

        public string Name()
        {
            return mNativeNode.Name.ToString();
        }
    }

    public class DBXmlDocument
    {
        private XDocument mNativeDoc;

        public DBXmlDocument(XDocument doc)
        {
            mNativeDoc = doc;
        }

        public DBXmlDocument()
        {
            mNativeDoc = new XDocument();
        }

        public void Load(string text)
        {
            mNativeDoc = XDocument.Parse(text);
        }

        public void Save(System.IO.Stream stream)
        {
            mNativeDoc.Save(stream);
        }

        public DBXmlNode Root()
        {
            return new DBXmlNode(mNativeDoc.Root);
        }

        public DBXmlNode AddRoot(string name)
        {
            XElement root = new XElement(name);
            mNativeDoc.Add(root);

            return new DBXmlNode(root);
        }

        public string OuterXml()
        {
            return mNativeDoc.ToString();
        }
    }
}
