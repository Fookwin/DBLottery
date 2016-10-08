package com.fookwin.lotterydata.util;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringReader;
import java.io.StringWriter;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

public class DBXmlDocument
{
    public DBXmlDocument(Document doc)
    {
        mNativeDoc = doc;
    }

    public DBXmlDocument()
    {

    }

    public boolean Load(String text)
    {
		try 
		{
	    	DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
			DocumentBuilder dbBuilder = dbFactory.newDocumentBuilder();
			StringReader sr=new StringReader(text);
			InputSource is=new InputSource(sr);
			   
			mNativeDoc = dbBuilder.parse(is);
	        
		} catch (ParserConfigurationException e){
			return false;
		}catch (SAXException e) {
			return false;
		} catch (IOException e) {
			return false;
		}
		
		return true;
    }

    public void Save(String filename)
    {
    	if (mNativeDoc == null)
    		return;
    	
        try {
            TransformerFactory tf = TransformerFactory.newInstance();
            Transformer transformer = tf.newTransformer();
            DOMSource source = new DOMSource(mNativeDoc);
            
            transformer.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
            transformer.setOutputProperty(OutputKeys.INDENT, "yes");
            
            PrintWriter pw = new PrintWriter(new FileOutputStream(filename));
            StreamResult result = new StreamResult(pw);
            transformer.transform(source, result);
        } catch (TransformerException mye) {
            mye.printStackTrace();
        } catch (IOException exp) {
            exp.printStackTrace();
        }   
    }

    public DBXmlNode Root()
    {
        return new DBXmlNode(_getDoc().getDocumentElement());
    }

    public DBXmlNode CreateNode(String name)
    {
        return new DBXmlNode(_getDoc().createElement(name));
    }

    public DBXmlNode AddRoot(String name)
    {
        return new DBXmlNode((Element)_getDoc().appendChild(_getDoc().createElement(name)));
    }

    private Document _getDoc()
    {
    	if (mNativeDoc == null)
    	{
        	DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();

            DocumentBuilder dbBuilder;
			try {
				dbBuilder = dbFactory.newDocumentBuilder();
			} catch (ParserConfigurationException e) {
				return null;
			}
            
            mNativeDoc = dbBuilder.newDocument();
    	}
    	
    	return mNativeDoc;
    }
    
    public String GetText()
    {
        String result = null;  

		if (mNativeDoc != null) {
			StringWriter strWtr = new StringWriter();
			StreamResult strResult = new StreamResult(strWtr);
			TransformerFactory tfac = TransformerFactory.newInstance();
			try {
				javax.xml.transform.Transformer t = tfac.newTransformer();
				t.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
				t.setOutputProperty(OutputKeys.INDENT, "yes");
				t.setOutputProperty(OutputKeys.METHOD, "xml");
				
				// text
				t.setOutputProperty("{http://xml.apache.org/xslt}indent-amount", "4");
				t.transform(new DOMSource(mNativeDoc.getDocumentElement()), strResult);
			} catch (Exception e) {
				System.err.println("XML.toString(Document): " + e);
			}
			
			result = strResult.getWriter().toString();
			
			try {
				strWtr.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		}

		return result;

    }
    
    private Document mNativeDoc;
}