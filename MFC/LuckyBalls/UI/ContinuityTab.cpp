// ContinuityTab.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "ContinuityTab.h"
#include "..\Data\Analysis\HistoryData.h"


// CContinuityTab dialog

IMPLEMENT_DYNAMIC(CContinuityTab, CDialog)

CContinuityTab::CContinuityTab(CWnd* pParent /*=NULL*/)
	: CDialog(CContinuityTab::IDD, pParent), m_pHistory(NULL)
{

}

CContinuityTab::~CContinuityTab()
{
}

void CContinuityTab::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_PROGRESS_0, m_Column0);
	DDX_Control(pDX, IDC_PROGRESS_1, m_Column1);
	DDX_Control(pDX, IDC_PROGRESS_2, m_Column2);
	DDX_Control(pDX, IDC_PROGRESS_3, m_Column3);
	DDX_Control(pDX, IDC_PROGRESS_4, m_Column4);
	DDX_Control(pDX, IDC_PROGRESS_5, m_Column5);
	DDX_Control(pDX, IDC_STATIC_STATE0, m_State0);
	DDX_Control(pDX, IDC_STATIC_STATE1, m_State1);
	DDX_Control(pDX, IDC_STATIC_STATE2, m_State2);
	DDX_Control(pDX, IDC_STATIC_STATE3, m_State3);
	DDX_Control(pDX, IDC_STATIC_STATE4, m_State4);
	DDX_Control(pDX, IDC_STATIC_STATE5, m_State5);
	DDX_Control(pDX, IDC_EDIT_START_ISSUE, m_editStartIssue);
	DDX_Control(pDX, IDC_EDIT_END_ISSUE, m_editEndIssue);
}


BEGIN_MESSAGE_MAP(CContinuityTab, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE, &CContinuityTab::OnBnClickedButtonUpdate)
END_MESSAGE_MAP()


// CContinuityTab message handlers
BOOL CContinuityTab::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	m_Column0.SetRange(0, 1000);
	m_Column1.SetRange(0, 1000);
	m_Column2.SetRange(0, 1000);
	m_Column3.SetRange(0, 1000);
	m_Column4.SetRange(0, 1000);
	m_Column5.SetRange(0, 1000);

	CString strTemp;
	const LuckyNums& nums = m_pHistory->GetHistory();
	int iStartIssue = nums.begin()->first;
	strTemp.Format(_T("%d"), iStartIssue);
	m_editStartIssue.SetWindowText(strTemp);

	int iEndIssue = nums.rbegin()->first;
	strTemp.Format(_T("%d"), iEndIssue);
	m_editEndIssue.SetWindowText(strTemp);

	Update();

	return TRUE;
}

void CContinuityTab::Update()
{
	CString strTemp;
	m_editStartIssue.GetWindowText(strTemp);
	int iStartIssue = _ttoi(strTemp);
	m_editEndIssue.GetWindowText(strTemp);
	int iEndIssue = _ttoi(strTemp);

	// Initialize the states;
	int iHit[6] = {0};
	int iTotal = 0;
	const LuckyNums& nums = m_pHistory->GetHistory();
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		if (it->first >= iStartIssue && it->first <= iEndIssue)
		{
			int iCon = it->second->GetContinuity();
			iHit[iCon] ++;
			iTotal ++;
		}
	}

	// Set control status.
	m_Column0.SetPos(iHit[0] * 1000 / iTotal);
	m_Column1.SetPos(iHit[1] * 1000 / iTotal);
	m_Column2.SetPos(iHit[2] * 1000 / iTotal);
	m_Column3.SetPos(iHit[3] * 1000 / iTotal);
	m_Column4.SetPos(iHit[4] * 1000 / iTotal);
	m_Column5.SetPos(iHit[5] * 1000 / iTotal);

	strTemp.Format(_T("%d"), iHit[0]);
	m_State0.SetWindowText(strTemp);
	strTemp.Format(_T("%d"), iHit[1]);
	m_State1.SetWindowText(strTemp);
	strTemp.Format(_T("%d"), iHit[2]);
	m_State2.SetWindowText(strTemp);
	strTemp.Format(_T("%d"), iHit[3]);
	m_State3.SetWindowText(strTemp);
	strTemp.Format(_T("%d"), iHit[4]);
	m_State4.SetWindowText(strTemp);
	strTemp.Format(_T("%d"), iHit[5]);
	m_State5.SetWindowText(strTemp);
}

void CContinuityTab::OnBnClickedButtonUpdate()
{
	Update();


	int sum[183 - 20] = {0};
	const LuckyNums& nums = m_pHistory->GetHistory();
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		sum[it->second->GetSum() - 21] ++;
	}

	CString strMsg, strTemp;
	for (int i = 1; i <= 163; i ++)
	{
		strTemp.Format(_T("%d : %d\t"), i + 20, sum[i - 1]);
		strMsg += strTemp;
		if (i%8 == 0)
			strMsg += _T("\n");
	}
	::MessageBox(NULL, strMsg, _T(""), MB_OK);
}
