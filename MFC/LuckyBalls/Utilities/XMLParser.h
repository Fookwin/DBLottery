// XMLParser.h: interface for the XMLParser class.
//
#pragma once
#import "msxml6.dll" named_guids

//  testse


class XMLNodeArray;
class XMLNode
{
	friend class XMLParser;
	friend class XMLNodeArray;
public:
	XMLNode();
	XMLNode(const XMLNode& src);
	XMLNode(MSXML2::IXMLDOMNode* Ptr);
	virtual ~XMLNode();

	MSXML2::IXMLDOMNode* GetNode() const;
	void SetNode(MSXML2::IXMLDOMNode* Ptr);

	XMLNode& operator= (const XMLNode& src);

	bool GetXml(BSTR* presult) const;
	bool GetXml(CComBSTR& result) const;
	bool CloneNode(XMLNode& result);

	bool SetNodeText(LPCTSTR text);
	bool SetNodeText(BSTR text);

	bool GetNodeText(CString& str) const;
	bool GetNodeText(CComBSTR& bstr) const;

	bool GetNodeName(CString& str) const;
	bool GetNodeName(CComBSTR& bstr) const;

	bool SetNodeValue(BSTR text);
	bool SetNodeValue(LPCTSTR text);

	bool GetNodeValue(CComBSTR& bstr) const;
	bool GetNodeValue(CString& str) const;

	XMLNode GetParentNode() const;
	bool IsNode() const;
	bool IsSameNode(const XMLNode& Node) const;

	bool SetProperty(LPCTSTR strId, VARIANT value);
	bool SetProperty(LPCTSTR strId, int value);
	bool SetProperty(LPCTSTR strId, bool value);
	bool SetProperty(LPCTSTR strId, BSTR value);
	bool SetProperty(LPCTSTR strId, LPCTSTR value);
	bool SetProperty(LPCTSTR strId, double dValue);
	bool SetProperty(LPCTSTR strId, long lValue);

	bool GetProperty(LPCTSTR strId, VARIANT* value) const;
	bool GetProperty(LPCTSTR strId, BSTR* pbstrVal) const;
	bool GetProperty(LPCTSTR strId, CComBSTR &Val) const;
	bool GetProperty(LPCTSTR strId, CString &Val) const;
	bool GetProperty(LPCTSTR strId, bool &Val) const;
	bool GetProperty(LPCTSTR strId, short &Val) const;
	bool GetProperty(LPCTSTR strId, double &Val) const;
	bool GetProperty(LPCTSTR strId, int &Val) const;
	bool GetProperty(LPCTSTR strId, long &Val) const;

	bool RemoveProperty(LPCTSTR strId);

	XMLNode AddNode(LPCTSTR name,LPCTSTR text=NULL,LPCTSTR attrName1=NULL,
		LPCTSTR attrVal1=NULL,LPCTSTR attrName2=NULL,LPCTSTR attrVal2=NULL);
	XMLNode AddNodeBefore(LPCTSTR name,LPCTSTR text=NULL,LPCTSTR attrName1=NULL,
		LPCTSTR attrVal1=NULL,LPCTSTR attrName2=NULL,LPCTSTR attrVal2=NULL);
	bool AddNode(const XMLNode& node);

	bool RemoveChildNode(LPCTSTR name);
	bool RemoveThisNode();

	bool GetChildNodes(XMLNodeArray &result) const;
	bool GetChildNames(CStringArray &result) const;
	bool GetChildNodes(LPCTSTR strId, XMLNodeArray &result) const;
	bool GetChildNode(LPCTSTR strId, XMLNode &result) const;
	long GetChildCount() const;

	bool GetProperties(CStringArray &result) const;

	void Empty();

	bool HasChildNodes() const;

private:
	MSXML2::IXMLDOMNodePtr m_pPtr;
};

inline MSXML2::IXMLDOMNode* XMLNode::GetNode() const
{
	return m_pPtr;
}

inline void XMLNode::SetNode(MSXML2::IXMLDOMNode* Ptr)
{
	m_pPtr = Ptr;
}

class XMLNodeArray: public CArray<XMLNode>
{
public:
	bool GetNodeNames(CStringArray &result) const;
};

class XMLParser
{
public:
	XMLParser();
	XMLParser(MSXML2::IXMLDOMDocument* pDoc);
	XMLParser(BSTR sourceXML);
	virtual ~XMLParser();

	MSXML2::IXMLDOMDocument* GetDocument();
	void SetDocument(MSXML2::IXMLDOMDocument* pDoc);

	void NewDocument();
	bool SetIdentifier(LPCTSTR str,bool bCreateNewFile=false);
	LPCTSTR GetIdentifier() const;
	bool LoadXmlDocument(LPCTSTR path);
	bool SaveXmlDocument(LPCTSTR path = NULL, bool bFormatXML = false) const;

	bool GetXml(BSTR* pbstr) const;
	bool GetXml(CComBSTR &str) const;
	bool SetXml(BSTR str);

	bool CreateXmlDocument(LPCTSTR szProcessingInstructionData = NULL);

	bool GetNode(LPCTSTR str,XMLNode &Node) const;
	bool GetNode(XMLNode &Node) const;
	bool GetNodesByTag(BSTR bszTag, XMLNodeArray &result) const;
	void RemoveNodesByTag(BSTR bszTag);
	long GetNodesCount() const;

	XMLNode GetRootNode() const;

	XMLNode AddNode(LPCTSTR name,LPCTSTR text=NULL,LPCTSTR attrName1=NULL,
		LPCTSTR attrVal1=NULL,LPCTSTR attrName2=NULL,LPCTSTR attrVal2=NULL);

protected:
	static void FormatXml(MSXML2::IXMLDOMDocumentPtr pDoc,MSXML2::IXMLDOMNodeListPtr list=NULL,int iIndent=0);

	MSXML2::IXMLDOMDocumentPtr m_pXmlDoc;

private:
	bool GenericXmlFilename(CString &s) const;
	MSXML2::IXMLDOMNodePtr CheckPtr(MSXML2::IXMLDOMNode* ptr) const;
};

inline MSXML2::IXMLDOMDocument* XMLParser::GetDocument()
{
	return m_pXmlDoc;
}

inline void XMLParser::SetDocument(MSXML2::IXMLDOMDocument* pDoc)
{
	m_pXmlDoc = pDoc;
}
