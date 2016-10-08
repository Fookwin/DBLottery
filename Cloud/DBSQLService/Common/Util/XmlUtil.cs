using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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

        public void SetValue(string value)
        {
            try
            {
                mNativeNode.InnerText = value;
            }
            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public string Value()
        {
            return mNativeNode.Value;
        }

        public List<DBXmlNode> ChildNodes()
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (XmlElement elm in mNativeNode.ChildNodes)
            {
                childnodes.Add(new DBXmlNode(elm));
            }

            return childnodes;
        }

        public List<DBXmlNode> ChildNodes(string name)
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (XmlElement elm in mNativeNode.ChildNodes)
            {
                if (elm.Name == name)
                    childnodes.Add(new DBXmlNode(elm));
            }

            return childnodes;
        }

        public DBXmlNode FirstChildNode(string name)
        {
            List<DBXmlNode> childnodes = new List<DBXmlNode>();

            foreach (XmlElement elm in mNativeNode.ChildNodes)
            {
                if (elm.Name == name)
                    return new DBXmlNode(elm);
            }

            return null;
        }

        public string Name()
        {
            return mNativeNode.Name;
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

        public void Load(string xml)
        {
            mNativeDoc.LoadXml(xml);
        }

        public void Save(string strFileName)
        {
            mNativeDoc.Save(strFileName);
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
            return mNativeDoc.OuterXml;
        }

        public string InnerXml()
        {
            return mNativeDoc.InnerXml;
        }

        private XmlDocument mNativeDoc;
    }
}
