// XMLParser.cpp: implementation of the XMLParser class.
//
#include "stdafx.h"
#include <locale.h>
#include "XMLParser.h"

// test....

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

// number of spaces for indent
#define TBNO 2

#define COMERR(X) HandleComError(_T(__FILE__),__LINE__,X)

static void HandleComError(LPCTSTR File, int LineNo, _com_error ce)
{
#ifdef _DEBUG
  CString str;
  str.Format(_T("File: %s\nLine: %d\nCom error message: %s\n"),File,LineNo,ce.ErrorMessage());
  AfxMessageBox(str);
#endif
}

///////////////////////////////////////////////////////////////////////////////////////////////////
XMLNode::XMLNode()
: m_pPtr(NULL)
{
}

XMLNode::XMLNode(const XMLNode& src)
: m_pPtr(src.m_pPtr)
{
}

XMLNode::XMLNode(MSXML2::IXMLDOMNode* Ptr)
: m_pPtr(Ptr)
{
}

XMLNode::~XMLNode()
{
}

XMLNode& XMLNode::operator= (const XMLNode& src)
{
  m_pPtr = src.m_pPtr;
  return *this;
}

bool XMLNode::HasChildNodes() const
{
	VARIANT_BOOL bHasChild = VARIANT_FALSE;
	bHasChild = m_pPtr->hasChildNodes();
	return bHasChild != VARIANT_FALSE;
}

long XMLNode::GetChildCount() const
{
  long lResult = 0;

  if (m_pPtr == NULL)
    return lResult;

  HRESULT hr;
  CComPtr<MSXML2::IXMLDOMNodeList> pNodeList;
  hr = m_pPtr->get_childNodes(&pNodeList);
  ATLASSERT(SUCCEEDED(hr));
  if (pNodeList == NULL) return lResult;

  hr = pNodeList->get_length(&lResult);
  return lResult;
}

bool XMLNode::RemoveChildNode(LPCTSTR name)
{
  XMLNode node;
  if(!GetChildNode(name,node)) return false;
  return node.RemoveThisNode();
}

bool XMLNode::RemoveThisNode()
{
	XMLNode parent=GetParentNode();
	if(!parent.IsNode()) return false;
	try
	{
		parent.m_pPtr->removeChild(m_pPtr);
		m_pPtr=NULL;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return true;
}

bool XMLNode::RemoveProperty(LPCTSTR strId)
{
	if(!IsNode()) return false;

	CComQIPtr<MSXML2::IXMLDOMElement> pElem;
	try
	{
		pElem=m_pPtr;
		pElem->removeAttribute(strId);
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return true;
}

bool XMLNode::GetXml(BSTR* presult) const
{
	if (*presult)
		::SysFreeString(*presult);

	if(!IsNode()) return false;

	HRESULT hr = m_pPtr->get_xml(presult);
	ATLASSERT(SUCCEEDED(hr));

	return (hr == S_OK);
}

bool XMLNode::GetXml(CComBSTR& result) const
{
	result.Empty();
	if(!IsNode()) return false;

	HRESULT hr = m_pPtr->get_xml(&result);
	ATLASSERT(SUCCEEDED(hr));

	return (hr == S_OK);
}

bool XMLNode::CloneNode(XMLNode& result)
{
	if(!IsNode()) return false;

	HRESULT hr = m_pPtr->raw_cloneNode(VARIANT_TRUE, &result.m_pPtr);
	ATLASSERT(SUCCEEDED(hr));

	return (hr == S_OK);
}

bool XMLNode::SetNodeValue(BSTR text)
{
  if(!IsNode()) return false;

  VARIANTARG value;
  ::VariantInit(&value);

  value.vt = VT_BSTR;
  value.bstrVal = text;

  HRESULT hr = m_pPtr->put_nodeValue(value);
  return hr == S_OK;
}

bool XMLNode::SetNodeValue(LPCTSTR text)
{
  if(!IsNode()) return false;

  CComVariant value(text);
  HRESULT hr = m_pPtr->put_nodeValue(value);
  return hr == S_OK;
}

bool XMLNode::GetNodeValue(CComBSTR& bstr) const
{
  bstr.Empty();
  if(!IsNode()) return false;

  VARIANTARG var;
  ::VariantInit(&var);

  HRESULT hr = m_pPtr->get_nodeValue(&var);
  if (FAILED(hr)) return false;

  if (var.vt != VT_BSTR) {
    ::VariantClear(&var);
    return false;
  }

  bstr.Attach(var.bstrVal); //attach the bstr and do not clear var
  return true;
}

bool XMLNode::GetNodeValue(CString& str) const
{
  if(!IsNode()) return false;

  CComVariant var;

  try {
	  m_pPtr->get_nodeValue(&var);
	  str=var;
	}
	catch(_com_error ce) {
	  COMERR(ce);
		return false;
	}
  return true;
}

bool XMLNode::SetNodeText(BSTR text)
{
  if(!IsNode()) return false;
  HRESULT hr = m_pPtr->put_text(text);
  return hr == S_OK;
}

bool XMLNode::SetNodeText(LPCTSTR text)
{
  if(!IsNode()) return false;
	try {
	  m_pPtr->put_text(_bstr_t(text));
	}
	catch(_com_error ce) {
	  COMERR(ce);
		return false;
	}
  return true;
}

bool XMLNode::GetNodeText(CString &str) const
{
  if(!IsNode()) return false;

  CComBSTR bs;
	try {
	  m_pPtr->get_text(&bs);
	  str=bs;
	}
	catch(_com_error ce) {
	  COMERR(ce);
		return false;
	}
  return true;
}

bool XMLNode::GetNodeText(CComBSTR& bstr) const
{
  if(!IsNode()) return false;

  HRESULT hr = m_pPtr->get_text(&bstr);
  return hr == S_OK;
}

bool XMLNode::GetNodeName(CString &str) const
{
  if(!IsNode()) return false;

  CComBSTR bs;
	try {
	  m_pPtr->get_nodeName(&bs);
	  str=bs;
	}
	catch(_com_error ce) {
	  COMERR(ce);
		return false;
	}
  return true;
}

bool XMLNode::GetNodeName(CComBSTR& bstr) const
{
  if(!IsNode()) return false;

  HRESULT hr = m_pPtr->get_nodeName(&bstr);
  return hr == S_OK;
}

XMLNode XMLNode::GetParentNode() const
{
	XMLNode result;
	if(m_pPtr==NULL) return result;
	m_pPtr->get_parentNode(&(result.m_pPtr));
	return result;
}

bool XMLNode::GetChildNode(LPCTSTR strId, XMLNode &result) const
{
  if(!IsNode()) return false;

  MSXML2::IXMLDOMNodePtr pChild;
  try
  {
    pChild = m_pPtr->selectSingleNode(strId);
    if (pChild)
    {
      result.m_pPtr=pChild;
      return true;
    }
  }
  catch(_com_error ce)
  {
    COMERR(ce);
    return false;
  }
  return false;
}

bool XMLNode::GetChildNodes(LPCTSTR strId, XMLNodeArray &result) const
{
  result.RemoveAll();

	if(!IsNode()) return false;

  CComPtr<MSXML2::IXMLDOMNodeList> pList;
  CComPtr<MSXML2::IXMLDOMNode> pChild;
  long i,iCount = 0L;

  HRESULT hr;
  hr = m_pPtr->raw_selectNodes(_bstr_t(strId), &pList);
  ATLASSERT(SUCCEEDED(hr));
  if (pList == NULL) return false;

  hr = pList->get_length(&iCount);
  ATLASSERT(SUCCEEDED(hr));

  result.SetSize(iCount);
  for(i=0;i<iCount;++i) {
    pChild.Release();
    hr = pList->get_item(i,&pChild);
    ATLASSERT(SUCCEEDED(hr));
    result.SetAt(i, XMLNode(pChild));
  }

  return (result.GetCount() > 0);
}

bool XMLNode::IsNode() const
{
	if(m_pPtr!=NULL) return true;
	return false;
}

bool XMLNode::IsSameNode(const XMLNode& Node) const
{
  CComPtr<MSXML2::IXMLDOMNode> pMyNode(m_pPtr);
  return (pMyNode.IsEqualObject(Node.m_pPtr)) ? true : false;
}

XMLNode XMLNode::AddNode(LPCTSTR name,LPCTSTR text,LPCTSTR attrName1,
						 LPCTSTR attrVal1,LPCTSTR attrName2,LPCTSTR attrVal2)
{
	MSXML2::IXMLDOMElementPtr pElem;
	MSXML2::IXMLDOMDocumentPtr pDoc;

	XMLNode result;

	if(!IsNode()) return result;

	try
	{
		pDoc=m_pPtr->GetownerDocument();
		if(pDoc==NULL) return result;

		pElem=pDoc->createElement(name);
		result.m_pPtr= m_pPtr->appendChild(pElem);

		if (text && *text) pElem->put_text(_bstr_t(text));
		if(attrName1 && attrVal1) pElem->setAttribute(_bstr_t(attrName1),_variant_t(attrVal1));
		if(attrName2 && attrVal2) pElem->setAttribute(_bstr_t(attrName2),_variant_t(attrVal2));
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}

	return result;
}

bool XMLNode::AddNode(const XMLNode& node)
{
	if(!IsNode() || !node.IsNode()) return false;

	try
	{
		m_pPtr->appendChild(node.m_pPtr);
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}

	return true;
}

XMLNode XMLNode::AddNodeBefore(LPCTSTR name,LPCTSTR text, LPCTSTR attrName1,
							   LPCTSTR attrVal1, LPCTSTR attrName2,LPCTSTR attrVal2)
{
	XMLNode result;
	MSXML2::IXMLDOMDocumentPtr pDoc;
	MSXML2::IXMLDOMElementPtr pElem;
	MSXML2::IXMLDOMNodePtr pRes;

	if(!IsNode()) return result;

	XMLNode ParentNode=GetParentNode();
	if(!ParentNode.IsNode()) return result;

	try
	{
		pDoc=m_pPtr->GetownerDocument();
		if(pDoc==NULL) return result;

		pElem=pDoc->createElement(name);
		if (text && *text) pElem->put_text(_bstr_t(text));
		if(attrName1 && attrVal1) pElem->setAttribute(_bstr_t(attrName1),_variant_t(attrVal1));
		if(attrName2 && attrVal2) pElem->setAttribute(_bstr_t(attrName2),_variant_t(attrVal2));
		pRes=ParentNode.m_pPtr->insertBefore(pElem,m_pPtr.GetInterfacePtr());
		result.m_pPtr=pElem;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}

	return result;
}

void XMLNode::Empty()
{
  m_pPtr = NULL;
}

bool XMLNode::SetProperty(LPCTSTR strId,int value)
{
	return SetProperty(strId,variant_t(long(value)));
}

bool XMLNode::SetProperty(LPCTSTR strId, double dValue)
{
  char* pCurLoc = setlocale(LC_NUMERIC,"C");

  bool bRet = false;
  static LPCTSTR format = _T("%.12f");
  int iBuffSize = _sctprintf(format,dValue);

  VARIANTARG varValue;
  ::VariantInit(&varValue);
  varValue.vt = VT_BSTR;

  varValue.bstrVal = ::SysAllocStringLen(NULL,iBuffSize);
  if (varValue.bstrVal == NULL)
  {
    setlocale(LC_NUMERIC,pCurLoc);
    return bRet;
  }

  _stprintf_s(varValue.bstrVal,iBuffSize+1,format,dValue);
  bRet = SetProperty(strId,varValue);

  ::SysFreeString(varValue.bstrVal);

  setlocale(LC_NUMERIC,pCurLoc);

  return bRet;
}

bool XMLNode::SetProperty(LPCTSTR strId, LPCTSTR szValue)
{
  if (!szValue)
    return false;

  CComQIPtr<MSXML2::IXMLDOMElement> pElem(m_pPtr);
  if (pElem == NULL) return false;

  HRESULT hr = pElem->raw_setAttribute(_bstr_t(strId),_variant_t(szValue));
  return hr == S_OK;
}

bool XMLNode::SetProperty(LPCTSTR strId,BSTR bstr)
{
  if (bstr == NULL)
    return false;

  CComQIPtr<MSXML2::IXMLDOMElement> pElem(m_pPtr);
  if (pElem == NULL) return false;

  VARIANTARG value;
  ::VariantInit(&value);
  value.vt = VT_BSTR;
  value.bstrVal = bstr;

  HRESULT hr = pElem->raw_setAttribute(_bstr_t(strId),value);

  return hr == S_OK;
}

bool XMLNode::SetProperty(LPCTSTR strId,bool value)
{
	return SetProperty(strId,	variant_t((value) ? VARIANT_TRUE : VARIANT_FALSE,VT_BOOL));
}

bool
XMLNode::SetProperty(LPCTSTR strId, long lValue)
{
	return SetProperty(strId,variant_t(lValue));
}

bool XMLNode::SetProperty(LPCTSTR strId, VARIANT value)
{
  CComQIPtr<MSXML2::IXMLDOMElement> pElem(m_pPtr);
  if (pElem == NULL) return false;

  HRESULT hr = pElem->raw_setAttribute(_bstr_t(strId),value);
  return hr == S_OK;
}

bool XMLNode::GetProperty(LPCTSTR strId, VARIANT* value) const
{
  CComQIPtr<MSXML2::IXMLDOMElement> pElem(m_pPtr);
  if (pElem == NULL) return false;

  HRESULT hr = pElem->raw_getAttribute(_bstr_t(strId),value);
  return hr == S_OK;
}

bool XMLNode::GetProperty(LPCTSTR strId, long &Val) const
{
	_variant_t value;
	if(!GetProperty(strId,&value)) return false;

	Val = (long)value;
	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId, BSTR* pbstrVal) const
{
	VARIANTARG value;
	::VariantInit(&value);
	if(!GetProperty(strId,&value)) return false;
	if(value.vt!=VT_BSTR)
	{
		::VariantClear(&value);
		return false;
	}

	*pbstrVal = value.bstrVal;

	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId, CComBSTR &Val) const
{
//  Val.Empty();

	VARIANTARG value;
	::VariantInit(&value);
	if(!GetProperty(strId,&value)) return false;
	if(value.vt!=VT_BSTR)
	{
		::VariantClear(&value);
		return false;
	}

	Val.Attach(value.bstrVal);

	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId,CString &strVal) const
{
//	strVal.Empty();

	variant_t value;
	if(!GetProperty(strId,&value)) return false;
	if(value.vt!=VT_BSTR) return false;

	strVal = value.bstrVal; //copy
	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId,double &dVal) const
{
//	dVal = 0.0;

	variant_t value;
	if(!GetProperty(strId,&value)) return false;

	char* pCurLoc = setlocale(LC_NUMERIC,"C");

	bool bRet;
	if (value.vt == VT_BSTR)
	{
		dVal = _tstof(value.bstrVal);
		bRet = true;
	}
	else
	{
		bRet = false;
	}

	setlocale(LC_NUMERIC,pCurLoc);
	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId,int &Val) const
{
//	Val=0;

	variant_t value;
	if(!GetProperty(strId,&value)) return false;
	try
	{
		Val=long(value);
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId,bool &bVal) const
{
	//	bVal=false;

	variant_t value;
	if (!GetProperty(strId,&value))
		return false;

	if (value.vt == VT_BOOL)
	{
		bVal = (value.boolVal != VARIANT_FALSE);
	}
	else if (value.vt == VT_BSTR)
	{
		HRESULT hr;
		VARIANT_BOOL bResult;
		hr = ::VarBoolFromStr(value.bstrVal,LOCALE_USER_DEFAULT,LOCALE_NOUSEROVERRIDE,&bResult);
		if (SUCCEEDED(hr))
			bVal = (bResult != VARIANT_FALSE);
		else
		{
			hr = ::VarBstrCmp(value.bstrVal,_T("true"),LOCALE_USER_DEFAULT,NORM_IGNORECASE);
			bVal = (hr == VARCMP_EQ) ? true : false;
		}
	}
	return true;
}

bool XMLNode::GetProperty(LPCTSTR strId,short &Val) const
{
//	Val=0;

	variant_t value;
	if(!GetProperty(strId,&value)) return false;
	try
	{
		Val=short(value);
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return true;
}

bool XMLNode::GetProperties(CStringArray &result) const
{
	result.RemoveAll();
	if(!IsNode()) return false;

	MSXML2::IXMLDOMNodePtr pChild;
	MSXML2::IXMLDOMNamedNodeMapPtr attrs;
	BSTR NameString;

	try
	{
		attrs=m_pPtr->Getattributes();
		result.SetSize(attrs->Getlength());
		for(long l=0;l<attrs->Getlength();++l)
		{
			pChild=attrs->Getitem(l);
			pChild->get_nodeName(&NameString);
			result.SetAt(l, NameString);
			::SysFreeString(NameString);
		}
		return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

bool XMLNode::GetChildNodes(XMLNodeArray &result) const
{
	result.RemoveAll();
	if(!IsNode()) return false;

	MSXML2::IXMLDOMNodePtr pChild;
	MSXML2::IXMLDOMNodeListPtr list;

	try
	{
		list=m_pPtr->GetchildNodes();
		result.SetSize(list->Getlength());
		for(long l=0;l<list->Getlength();++l)
		{
			pChild=list->Getitem(l);
			result.SetAt(l, XMLNode(pChild));
		}
		return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

bool XMLNode::GetChildNames(CStringArray &result) const
{
	result.RemoveAll();
	if(!IsNode()) return false;

	MSXML2::IXMLDOMNodePtr pChild;
	MSXML2::IXMLDOMNodeListPtr list;
	BSTR NameString;

	try
	{
		list=m_pPtr->GetchildNodes();
		result.SetSize(list->Getlength());
		for(long l=0;l<list->Getlength();++l)
		{
			pChild=list->Getitem(l);
			pChild->get_nodeName(&NameString);
			result.SetAt(l, NameString);
			::SysFreeString(NameString);
		}
		return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

bool XMLNodeArray::GetNodeNames(CStringArray &result) const
{
	result.RemoveAll();

	MSXML2::IXMLDOMNodePtr pChild;
	BSTR NameString;

	try
	{
		result.SetSize(GetSize());
		for(int i=0;i<GetSize();++i)
		{
			pChild=GetAt(i).m_pPtr;
			pChild->get_nodeName(&NameString);
			result.SetAt(i,NameString);
			::SysFreeString(NameString);
		}
		return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

///////////////////////////////////////////////////////////////////////////////////////////////////
XMLParser::XMLParser()
{
	CreateXmlDocument();
}

XMLParser::XMLParser(MSXML2::IXMLDOMDocument* pDoc)
: m_pXmlDoc(pDoc)
{
}

XMLParser::XMLParser(BSTR sourceXML)
{
	HRESULT hr;
	hr = m_pXmlDoc.CreateInstance(MSXML2::CLSID_DOMDocument60);
	ATLASSERT(SUCCEEDED(hr));
	if (m_pXmlDoc == NULL) return;

	VARIANT_BOOL bResult= VARIANT_FALSE;
	hr = m_pXmlDoc->raw_loadXML(sourceXML,&bResult);
	ATLASSERT(SUCCEEDED(hr) && bResult != VARIANT_FALSE);
}

XMLParser::~XMLParser()
{
}

void XMLParser::FormatXml(MSXML2::IXMLDOMDocumentPtr pDoc,MSXML2::IXMLDOMNodeListPtr list,int iIndent)
{
	MSXML2::IXMLDOMNodePtr pPar,pSibling,pChild;
	MSXML2::IXMLDOMTextPtr txt;
	CString str;
	MSXML2::IXMLDOMElementPtr pElem;

	try
	{
		if(list==NULL)
		{
			pElem=pDoc->documentElement;
			if(pElem!=NULL)
			{
				list=pElem->GetchildNodes();
			}
			if(list==NULL) return;
		}

		for(long l=0;l<list->Getlength();++l)
		{
			pChild=list->Getitem(l);
			if(pChild->GetnodeType()==MSXML2::NODE_TEXT)
			{
				continue;
			}
			str = _T("\n                                                                              ");
			str = str.Left(1+(iIndent+1)*TBNO);
			txt = pDoc->createTextNode(LPCTSTR(str));
			pPar = pChild->GetparentNode();
			pPar->insertBefore(txt,pChild.GetInterfacePtr());
			txt.Release();

			pSibling = pChild->GetnextSibling();
			if(pSibling==NULL)
			{
				str = _T("\n                                                                              ");
				str = str.Left(1+iIndent*TBNO);
				txt = pDoc->createTextNode(LPCTSTR(str));
				pPar = pChild->GetparentNode();
				pPar->appendChild(txt);
				txt.Release();
			}
			l+=1;
			FormatXml(pDoc,pChild->GetchildNodes(),iIndent+1);
		}
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return;
	}
}

MSXML2::IXMLDOMNodePtr XMLParser::CheckPtr(MSXML2::IXMLDOMNode* ptr) const
{
	MSXML2::IXMLDOMNodePtr pResult;
	if (!ptr)
	{
		try
		{
			if (m_pXmlDoc != NULL)
			{
				pResult = m_pXmlDoc->documentElement;
			}
		}
		catch (_com_error&)
		{
			pResult = NULL;
		}

		if(pResult==NULL)
		{
			pResult = m_pXmlDoc;
		}
	}
	else
	{
		pResult = ptr;
	}

	return pResult;
}

XMLNode XMLParser::AddNode(LPCTSTR name,LPCTSTR text,LPCTSTR attrName1,
						   LPCTSTR attrVal1,LPCTSTR attrName2,LPCTSTR attrVal2)
{
	XMLNode result;
	if(m_pXmlDoc==NULL) return result;

	MSXML2::IXMLDOMElementPtr pElem;
	MSXML2::IXMLDOMNodePtr pPtr=CheckPtr(NULL);

	try
	{
		pElem = m_pXmlDoc->createElement(name);
		result.m_pPtr = pPtr->appendChild(pElem);
		if (text && *text) pElem->put_text(_bstr_t(text));
		if(attrName1 && attrVal1) pElem->setAttribute(_bstr_t(attrName1),_variant_t(attrVal1));
		if(attrName2 && attrVal2) pElem->setAttribute(_bstr_t(attrName2),_variant_t(attrVal2));
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return result;
	}
	return result;
}

bool XMLParser::CreateXmlDocument(LPCTSTR szProcessingInstructionData/* = NULL*/)
{
	if(m_pXmlDoc)
		m_pXmlDoc.Release();

	HRESULT hr = m_pXmlDoc.CreateInstance(MSXML2::CLSID_DOMDocument60);
	ATLASSERT(SUCCEEDED(hr));
	if (m_pXmlDoc == NULL) return false;

	MSXML2::IXMLDOMProcessingInstructionPtr pInstr;
	MSXML2::IXMLDOMElementPtr pPackage;

	try
	{
		_bstr_t bstrProcessingInstruction(szProcessingInstructionData);
		if (bstrProcessingInstruction.length() < 1)
			bstrProcessingInstruction = _T("version='1.0' encoding='UTF-16'");
		pInstr = m_pXmlDoc->createProcessingInstruction(_bstr_t(_T("xml")), bstrProcessingInstruction);
		//pInstr = m_pXmlDoc->createProcessingInstruction(_T("xml"),_T("version='1.0' encoding='UTF-8'"));
		pPackage = m_pXmlDoc->documentElement;
		if(pPackage!=NULL)
		{
			_variant_t firstchild(pPackage.GetInterfacePtr());
			m_pXmlDoc->insertBefore(pInstr,firstchild);
		}
		else
		{
			m_pXmlDoc->appendChild(pInstr);
		}
		return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

bool XMLParser::GetNode(LPCTSTR strId,XMLNode &result) const
{
	result.m_pPtr=NULL;

	MSXML2::IXMLDOMNodePtr pPtr=CheckPtr(NULL);
	BSTR NameString;

	try {
		if(pPtr==NULL) return false;
		for(;pPtr!=NULL;pPtr=pPtr->GetnextSibling())
		{
			if(!strId)
			{
				result.m_pPtr=pPtr;
				return true;
			}
			pPtr->get_nodeName(&NameString);
			if(!_tcscmp(strId,NameString))
			{
				result.m_pPtr=pPtr;
				::SysFreeString(NameString);
				return true;
			}
			::SysFreeString(NameString);
		}
		return false;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
}

bool XMLParser::GetNode(XMLNode &result) const
{
	result.m_pPtr=NULL;
	MSXML2::IXMLDOMNodePtr pPtr=CheckPtr(NULL);
	if(pPtr==NULL) return false;
	result.m_pPtr=pPtr;
	return true;
}

bool XMLParser::SaveXmlDocument(LPCTSTR path, bool bFormatXML) const
{
	if(m_pXmlDoc==NULL) return false;

	CString s;
	if (path)
	{
		s = path;
	}
	else
	{
		if(!GenericXmlFilename(s)) return false;
	}

	DeleteFile(s);
	if(bFormatXML)
		FormatXml(m_pXmlDoc);

	try
	{
		if(SUCCEEDED(m_pXmlDoc->save(LPCTSTR(s)))) return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return false;
}

bool XMLParser::LoadXmlDocument(LPCTSTR path)
{
	if(m_pXmlDoc==NULL) return false;

	DWORD res=GetFileAttributes(path);
	if(res==0xFFFFFFFF) return false;

	try
	{
		if(m_pXmlDoc->load(path) != VARIANT_FALSE) return true;
	}
	catch(_com_error& ce)
	{
		COMERR(ce);
		return false;
	}
	return false;
}

bool XMLParser::GenericXmlFilename(CString &s) const
{
	::GetTempPath(MAX_PATH,s.GetBuffer(MAX_PATH+2));
	s.ReleaseBuffer();

	if (s.GetAt(s.GetLength()-1)!= _T('\\'))
		s += _T("\\");

	GUID guid;
	CoCreateGuid(&guid);

	OLECHAR szGUID[64];
	::StringFromGUID2(guid,szGUID,64); 
	s += szGUID;

	s += _T(".xml");
	return true;
}

bool XMLParser::GetXml(BSTR* pbstr) const
{
	if (m_pXmlDoc==NULL)
		return false;

	if (*pbstr)
		::SysFreeString(*pbstr);
	HRESULT hr;
	hr = m_pXmlDoc->get_xml(pbstr);
	ATLASSERT(SUCCEEDED(hr));

	return (hr == S_OK);
}

bool XMLParser::GetXml(CComBSTR& bstr) const
{
  if (m_pXmlDoc==NULL)
    return false;

  bstr.Empty();
  HRESULT hr;
  hr = m_pXmlDoc->get_xml(&bstr);
  ATLASSERT(SUCCEEDED(hr));

  return (hr == S_OK);
}

bool XMLParser::SetXml(BSTR str)
{
	if (m_pXmlDoc==NULL)
		return false;

	HRESULT hr;
	VARIANT_BOOL bResult = VARIANT_FALSE;
	hr = m_pXmlDoc->raw_loadXML(str, &bResult);
	ATLASSERT(SUCCEEDED(hr));
	if (FAILED(hr)) return false;

	return (bResult != VARIANT_FALSE);
}

bool XMLParser::GetNodesByTag(BSTR bszTag, XMLNodeArray &result) const
{
	result.RemoveAll();

	if (m_pXmlDoc == NULL)
		return false;

	HRESULT hr;
	CComPtr<MSXML2::IXMLDOMNodeList> pNodeList;

	hr = m_pXmlDoc->raw_getElementsByTagName(bszTag,&pNodeList);
	ATLASSERT(SUCCEEDED(hr));
	if (hr != S_OK) return false;

	long i,iCount = 0;
	hr = pNodeList->get_length(&iCount);
	hr = pNodeList->raw_reset();

	result.SetSize(iCount);
	CComPtr<MSXML2::IXMLDOMNode> pNode;
	for (i=0;i<iCount;++i)
	{
		hr = pNodeList->get_item(i,&pNode);
		ATLASSERT(SUCCEEDED(hr));
		if (pNode != NULL)
		{
			result.SetAt(i, XMLNode(pNode));
		}
		pNode.Release();
	}
	return true;
}

void XMLParser::RemoveNodesByTag(BSTR bszTag)
{
  HRESULT hr;

  if (m_pXmlDoc == NULL)
    return;

  CComPtr<MSXML2::IXMLDOMNodeList> pNodeList;
  hr = m_pXmlDoc->raw_getElementsByTagName(bszTag,&pNodeList);
  ATLASSERT(SUCCEEDED(hr));
  if (pNodeList == NULL) return;

  long i,iCount = 0;
  hr = pNodeList->get_length(&iCount);
  hr = pNodeList->raw_reset();

  CComPtr<MSXML2::IXMLDOMNode> pNode;
  CComPtr<MSXML2::IXMLDOMNode> pOldChild;
  CComPtr<MSXML2::IXMLDOMNode> pParentNode;

  for (i=0;i<iCount;++i) {
    hr = pNodeList->get_item(i,&pNode);
    ATLASSERT(SUCCEEDED(hr));
    if (pNode == NULL) continue;

    hr = pNode->get_parentNode(&pParentNode);
    if (pParentNode != NULL) {
      pOldChild.Release();
      pParentNode->raw_removeChild(pNode,&pOldChild);
    }
    pNode.Release();
  }
}

long XMLParser::GetNodesCount() const
{
  long lResult = 0;
  if (m_pXmlDoc == NULL)
    return lResult;

  HRESULT hr;
  CComPtr<MSXML2::IXMLDOMElement> pElement;
  hr = m_pXmlDoc->get_documentElement(&pElement);
  ATLASSERT(SUCCEEDED(hr));
  if (pElement == NULL)
    return lResult;

  CComPtr<MSXML2::IXMLDOMNodeList> pNodeList;
  hr = pElement->get_childNodes(&pNodeList);
  ATLASSERT(SUCCEEDED(hr));
  if (pNodeList == NULL)
    return lResult;

  pNodeList->get_length(&lResult);
  return lResult;
}

XMLNode XMLParser::GetRootNode() const
{
  return XMLNode(CheckPtr(NULL));
}
