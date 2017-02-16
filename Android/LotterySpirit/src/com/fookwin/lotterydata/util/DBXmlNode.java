package com.fookwin.lotterydata.util;

import java.util.ArrayList;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

public class DBXmlNode
{
    public DBXmlNode(Element node)
    {
        mNativeNode = node;
    }

    public DBXmlDocument OwnerDocument()
    {
        return new DBXmlDocument(mNativeNode.getOwnerDocument());
    }

    public DBXmlNode AddChild(String name)
    {
    	Document doc = mNativeNode.getOwnerDocument();
    	
        return new DBXmlNode((Element)mNativeNode.appendChild(doc.createElement(name)));
    }

    public String GetAttribute(String name)
    {
        return mNativeNode.getAttribute(name);
    }

    public void SetAttribute(String name, String value)
    {
        mNativeNode.setAttribute(name, value);
    }

    public ArrayList<DBXmlNode> ChildNodes()
    {
    	ArrayList<DBXmlNode> childnodes = new ArrayList<DBXmlNode>();

        NodeList nodeList = mNativeNode.getChildNodes();
        for (int i = 0; i < nodeList.getLength(); ++ i)
        {
        	Node node = nodeList.item(i);
        	if (node instanceof Element)
        	{
	            Element element = (Element)node;
	            if (element != null)
	                childnodes.add(new DBXmlNode(element));
        	}
        }

        return childnodes;
    }

    public ArrayList<DBXmlNode> ChildNodes(String nodeName)
    {
    	ArrayList<DBXmlNode> childnodes = new ArrayList<DBXmlNode>();
    	
    	NodeList nodeList = mNativeNode.getElementsByTagName(nodeName);
        for (int i = 0; i < nodeList.getLength(); ++ i)
        {
        	Node node = nodeList.item(i);
        	if (node instanceof Element)
        	{
	            Element element = (Element)node;
	            childnodes.add(new DBXmlNode(element));
        	}
        }

        return childnodes;
    }

    public DBXmlNode FirstChildNode(String nodeName)
    {
    	NodeList nodeList = mNativeNode.getElementsByTagName(nodeName);
    	if (nodeList.getLength() > 0)
    		return new DBXmlNode((Element)nodeList.item(0));

        return null;
    }

    public DBXmlNode FirstChildNode()
    {
    	Node node = mNativeNode.getFirstChild();
    	if (node instanceof Element)
    	{            
            return new DBXmlNode((Element)node);
    	}
    	
    	return null;
    }

    public String Name()
    {
        return mNativeNode.getTagName();
    }

    private Element mNativeNode;
}