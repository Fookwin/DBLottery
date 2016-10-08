// C:\My Projects\Lottery\LuckyBalls\LuckyBalls\UI\Controls\SelectNumDlg.cpp : implementation file
//

#include "stdafx.h"
#include "..\..\LuckyBalls.h"
#include "SelectNumDlg.h"
#include <math.h>

#define DEFAULT_BUTTON_WIDTH		87
#define DEFAULT_BUTTON_WIDTH_MIN	30
#define DEFAULT_BUTTON_HEIGHT		24
#define DEFAULT_MARGIN				4
#define DEFAULT_SPACE				2

#define CAL_BLOCK_INDEX(num_inx, block_size) (((num_inx) == 0 || (block_size) == 0) ? 0 : ((int)(num_inx)/(block_size) + ((num_inx)%(block_size)==0 ? 0 : 1)))

// CSelectNumDlg dialog

IMPLEMENT_DYNAMIC(CSelectNumDlg, CDialog)

CSelectNumDlg::CSelectNumDlg(CNumSet& init, CRagion ragion, int iUnitRagion /*= 1*/, 
							 const CString& strCapture /*= _T("Select Numbers")*/,
							 CWnd* pParent /*=NULL*/)
	: CDialog(CSelectNumDlg::IDD, pParent), m_numSet(init), m_iUnitRagion(iUnitRagion), m_Ragion(ragion),
	m_strTitle(strCapture)
{
	ASSERT(iUnitRagion > 0);

	if (::IsWindow(pParent->GetSafeHwnd()))
	{
		CRect rectParent;
		pParent->GetWindowRect(rectParent);

		m_Position.x = rectParent.right;
		m_Position.y = rectParent.top;
	}
}

CSelectNumDlg::~CSelectNumDlg()
{
	for (std::vector<CButton*>::iterator it = m_Buttons.begin(); it != m_Buttons.end(); it ++)
	{
		delete (*it);
	}
	m_Buttons.clear();
}

void CSelectNumDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_TEXT, m_EditText);
}


BEGIN_MESSAGE_MAP(CSelectNumDlg, CDialog)
	ON_EN_CHANGE(IDC_EDIT_TEXT, &CSelectNumDlg::OnEnChangeEditText)
	ON_BN_CLICKED(IDC_CHECK1, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK2, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK3, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK4, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK5, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK6, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK7, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK8, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK9, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK10, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK11, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK12, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK13, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK14, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK15, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK16, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK17, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK18, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK19, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK20, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK21, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK22, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK23, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK24, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK25, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK26, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK27, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK28, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK29, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK30, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK31, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK32, &CSelectNumDlg::OnBnClickedCheck)
	ON_BN_CLICKED(IDC_CHECK33, &CSelectNumDlg::OnBnClickedCheck)
END_MESSAGE_MAP()		   
						   
						   
// CSelectNumDlg message handlers
BOOL CSelectNumDlg::OnInitDialog()
{						   
	__super::OnInitDialog();
						   
	CRect rect;
	GetWindowRect(rect);

	rect.left = m_Position.x;
	rect.top = m_Position.y;
	
	int iBtnNum = CAL_BLOCK_INDEX(m_Ragion.m_iStep + 1, m_iUnitRagion);
	int iColumnCount = max((int)sqrt((double)iBtnNum) + 1, m_iUnitRagion == 1 ? 6 : 2);
	int iLineCount = CAL_BLOCK_INDEX(iBtnNum, iColumnCount);
	int iBtnWidth = m_iUnitRagion == 1 ? DEFAULT_BUTTON_WIDTH_MIN : DEFAULT_BUTTON_WIDTH;

	rect.right = rect.left + max(iColumnCount * (iBtnWidth + DEFAULT_SPACE), DEFAULT_BUTTON_WIDTH * 2) + DEFAULT_MARGIN * 2;
	rect.bottom = rect.top + (iLineCount + 2) * (DEFAULT_BUTTON_HEIGHT + DEFAULT_SPACE) + DEFAULT_MARGIN * 3;

	MoveWindow(rect);

	// Move text editor.
	CRect rectText;
	rectText.top = DEFAULT_MARGIN;
	rectText.left = DEFAULT_MARGIN;
	rectText.bottom = rectText.top + DEFAULT_BUTTON_HEIGHT;
	rectText.right = rectText.left + rect.Width() - DEFAULT_MARGIN * 2;
	m_EditText.MoveWindow(rectText);

	// Move ok button.
	CWnd* pOkBtn = (CWnd*)GetDlgItem(IDOK);
	CRect rectOk;
	pOkBtn->GetWindowRect(rectOk);

	CRect rectOkNew;
	rectOkNew.left = rect.Width() - DEFAULT_MARGIN - DEFAULT_SPACE - rectOk.Width() * 2;
	rectOkNew.right = rectOkNew.left + rectOk.Width();
	rectOkNew.top = rect.Height() - DEFAULT_MARGIN - rectOk.Height();
	rectOkNew.bottom = rectOkNew.top + rectOk.Height();

	pOkBtn->MoveWindow(rectOkNew);

	// Move cancel button.
	CWnd* pCancelBtn = (CWnd*)GetDlgItem(IDCANCEL);
	CRect rectCancel;
	pCancelBtn->GetWindowRect(rectCancel);

	CRect rectCancelNew;
	rectCancelNew.left = rect.Width() - DEFAULT_MARGIN - rectCancel.Width();
	rectCancelNew.right = rectCancelNew.left + rectCancel.Width();
	rectCancelNew.top = rect.Height() - DEFAULT_MARGIN - rectCancel.Height();
	rectCancelNew.bottom = rectCancelNew.top + rectCancel.Height();

	pCancelBtn->MoveWindow(rectCancelNew);

	// Set window text.
	SetWindowText(m_strTitle);

	// Initialize the number controls.
	int iIndex = 0;
	int iMinNum = m_Ragion.m_iMin;
	int iPosX = rect.left + 4, iPosY = rect.top + 4;
	for (std::vector<CButton*>::iterator it = m_Buttons.begin(); it != m_Buttons.end(); it ++, iIndex ++)
	{
		if (iIndex < iBtnNum)
		{
			// Set button state.
			UpdateButtonState(*it, iIndex, iMinNum, iColumnCount, iBtnWidth);
			(*it)->SetCheck(BST_UNCHECKED);
			
			// show this button.
			(*it)->ShowWindow(SW_SHOW);
		}
		else
		{
			// Hide this button.
			(*it)->ShowWindow(SW_HIDE);
		}
	}

	for (; iIndex < iBtnNum; iIndex ++)
	{
		// Add new button.
		CButton* pNewBtn = new CButton();
		pNewBtn->Create( _T(""), WS_CHILD | WS_VISIBLE | BS_PUSHBUTTON | BS_AUTOCHECKBOX | BS_PUSHLIKE | WS_VISIBLE | WS_TABSTOP,
			rect, this, WM_USER + 0x0100 + iIndex);

		// Set button state.
		UpdateButtonState(pNewBtn, iIndex, iMinNum, iColumnCount, iBtnWidth);
		pNewBtn->SetCheck(BST_UNCHECKED);

		m_Buttons.push_back(pNewBtn);
	}

	// Update the number set.
	SetNumSet(m_numSet);

	// update edit text
	m_EditText.SetWindowText(m_numSet.GetText());

	return TRUE;
}

bool CSelectNumDlg::UpdateButtonState(CButton* pBtn, int iIndex, int& iMin,
									  int iColCount, int iBtnWidth)
{
	if (pBtn == NULL || iIndex < 0) return false;

	// Reset the button.
	CString str;
	if (m_iUnitRagion == 1)
	{
		str.Format(_T("%d"), iMin ++);
	}
	else
	{
		int iMax = min(iMin + m_iUnitRagion - 1, m_Ragion.m_iMin + m_Ragion.m_iStep);
		if (iMax == iMin)
			str.Format(_T("%d"), iMin);
		else
			str.Format(_T("%d ~ %d"), iMin, iMax);

		iMin = iMax + 1;
	}

	pBtn->SetWindowText(str);

	// Set position.
	CRect rectBtn;
	rectBtn.left = DEFAULT_MARGIN + (iBtnWidth + DEFAULT_SPACE) * (iIndex % iColCount);
	rectBtn.right = rectBtn.left + iBtnWidth;
	rectBtn.top = DEFAULT_MARGIN + (DEFAULT_BUTTON_HEIGHT + DEFAULT_SPACE) * (iIndex / iColCount + 1);
	rectBtn.bottom = rectBtn.top + DEFAULT_BUTTON_HEIGHT;

	pBtn->MoveWindow(rectBtn);

	return true;
}

void CSelectNumDlg::OnOK()
{
	// Update number set.
	m_numSet.Clear();

	// Update the num set.
	UpdateNumSet();

	return __super::OnOK();
}

bool CSelectNumDlg::UpdateNumSet()
{
	// Update the num set.
	m_numSet.Clear();

	int iIndex = 0;
	bool bInit = false;
	CRagion previous(m_Ragion.m_iMin, m_iUnitRagion - 1);
	for (std::vector<CButton*>::iterator it = m_Buttons.begin(); it != m_Buttons.end(); it ++, iIndex ++)
	{
		if ((*it) != NULL && (*it)->GetCheck())
		{
			if (!bInit)
			{
				previous.m_iMin = m_Ragion.m_iMin + iIndex * m_iUnitRagion;
				bInit = true;
				continue;
			}

			if (previous.m_iMin + previous.m_iStep + 1== m_Ragion.m_iMin + iIndex * m_iUnitRagion)
			{
				// Combine with previous.
				previous.m_iStep += m_iUnitRagion;
			}
			else
			{
				// Submit the preivous and reset the region.
				m_numSet.AddNum(previous);

				previous.m_iMin = m_Ragion.m_iMin + iIndex * m_iUnitRagion;
				previous.m_iStep = m_iUnitRagion - 1;
			}
		}
	}

	if (bInit)
	{
		// Submit the last.
		m_numSet.AddNum(previous);
	}

	// update edit text
	m_EditText.SetWindowText(m_numSet.GetText());

	return true;
}

bool CSelectNumDlg::SetNumSet(const CNumSet& numSet)
{
	for (std::vector<CButton*>::iterator it = m_Buttons.begin(); it != m_Buttons.end(); it ++)
	{
		(*it)->SetCheck(BST_UNCHECKED);
	}

	const Ragions& ragions = numSet.GetNums();
	for (Ragions::const_iterator it = ragions.begin(); it != ragions.end(); it ++)
	{
		// Calcuate the index of the button.
		int iStartIndex = CAL_BLOCK_INDEX(it->m_iMin - m_Ragion.m_iMin + 1, m_iUnitRagion) - 1;
		int iEndIndex = CAL_BLOCK_INDEX(it->m_iMin + it->m_iStep - m_Ragion.m_iMin + 1, m_iUnitRagion) - 1;
		ASSERT(iStartIndex >= 0 && iStartIndex <= iEndIndex && iEndIndex < (int)m_Buttons.size());
		if (iStartIndex < 0 || iStartIndex > iEndIndex || iEndIndex > (int)m_Buttons.size())
			return false;

		for (int i = iStartIndex; i <= iEndIndex; i ++)
		{
			CButton* pBtn = m_Buttons[i];
			if (pBtn != NULL)
				pBtn->SetCheck(BST_CHECKED);
		}
	}

	return true;
}


void CSelectNumDlg::OnEnChangeEditText()
{
	CString text;
	m_EditText.GetWindowText(text);

	CString strPrevious = m_numSet.GetText();
	if (text != strPrevious)
	{
		if (!m_numSet.SetText(text))
		{
			m_numSet.SetText(strPrevious);
		}
		else
		{		
			SetNumSet(m_numSet);
		}
	}
}

void CSelectNumDlg::OnBnClickedCheck()
{
	UpdateNumSet();
}