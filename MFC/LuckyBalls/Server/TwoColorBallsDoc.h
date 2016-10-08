
// TwoColorBallsDoc.h : interface of the CTwoColorBallsDoc class
//


#pragma once

class CHistoryData;
class CTwoColorBallsDoc : public CDocument
{
protected: // create from serialization only
	CTwoColorBallsDoc();
	DECLARE_DYNCREATE(CTwoColorBallsDoc)

// Attributes
public:
	CHistoryData* GetHistoryData() const { return m_pData; };
	bool IsInitialized() const { return m_bInitialized; }

// Operations
public:

// Overrides
public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

// Implementation
public:
	virtual ~CTwoColorBallsDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()

	CHistoryData* m_pData;
	bool m_bInitialized;
};


