using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;

namespace LuckyBallsData.Util
{
    public class DBXmlNode
    {
        public DBXmlNode(XmlElement node)
        {
            mNativeNode = node;
        }

        public DBXmlDocument OwnerDocument()
        {
            return new DBXmlDocument(mNativeNode.OwnerDocument);
        }

        public DBXmlNode AddChild(string name)
        {
            return new DBXmlNode(mNativeNode.AppendChild(mNativeNode.OwnerDocument.CreateElement(name)) as XmlElement);
        }

        public string GetAttribute(string name)
        {
            return mNativeNode.GetAttribute(name);
        }

        public void SetAttribute(string name, string value)
        {
            mNativeNode.SetAttribute(name, value);
        }

        public List<DBXmlNode> ChildNodes()
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (var sub in mNativeNode.ChildNodes)
            {
                XmlElement element = sub as XmlElement;
                if (element != null)
                    childnodes.Add(new DBXmlNode(element));
            }

            return childnodes;
        }

        public List<DBXmlNode> ChildNodes(string nodeName)
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (var sub in mNativeNode.ChildNodes)
            {
                XmlElement element = sub as XmlElement;
                if (element != null && element.NodeName == nodeName)
                    childnodes.Add(new DBXmlNode(element));
            }

            return childnodes;
        }

        public DBXmlNode FirstChildNode(string nodeName)
        {
            foreach (var sub in mNativeNode.ChildNodes)
            {
                XmlElement element = sub as XmlElement;
                if (element != null && element.NodeName == nodeName)
                    return new DBXmlNode(element);
            }

            return null;
        }

        public DBXmlNode FirstChildNode()
        {
            return new DBXmlNode(mNativeNode.FirstChild as XmlElement);
        }

        public string Name()
        {
            return mNativeNode.TagName;
        }

        private XmlElement mNativeNode;
    }

    public class DBXmlDocument
    {
        public DBXmlDocument(XmlDocument doc)
        {
            mNativeDoc = doc;
        }

        public DBXmlDocument()
        {
            mNativeDoc = new XmlDocument();
        }

        public void Load(string text)
        {
            mNativeDoc.LoadXml(text);
        }

        public async Task Save(Windows.Storage.IStorageFile file)
        {
            await mNativeDoc.SaveToFileAsync(file);
        }

        public DBXmlNode Root()
        {
            return new DBXmlNode(mNativeDoc.FirstChild as XmlElement);
        }

        public DBXmlNode CreateNode(string name)
        {
            return new DBXmlNode(mNativeDoc.CreateElement(name));
        }

        public DBXmlNode AddRoot(string name)
        {
            return new DBXmlNode(mNativeDoc.AppendChild(mNativeDoc.CreateElement(name)) as XmlElement);
        }

        public string OuterXml()
        {
            return mNativeDoc.GetXml();
        }

        public string InnerXml()
        {
            return mNativeDoc.InnerText;
        }

        private XmlDocument mNativeDoc;
    }
}
