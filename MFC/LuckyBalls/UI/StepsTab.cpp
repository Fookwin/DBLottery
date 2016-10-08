// ContinuityTab.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "StepsTab.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Utilities\AnalysisUtil.h"


// CStepsTab dialog

IMPLEMENT_DYNAMIC(CStepsTab, CDialog)

CStepsTab::CStepsTab(CWnd* pParent /*=NULL*/)
	: CDialog(CStepsTab::IDD, pParent), m_pHistory(NULL), m_iMaxHitCount(0)
{
	Clear();
}

CStepsTab::~CStepsTab()
{
}

void CStepsTab::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_PROGRESS1, m_Column1);
	DDX_Control(pDX, IDC_PROGRESS2, m_Column2);
	DDX_Control(pDX, IDC_PROGRESS3, m_Column3);
	DDX_Control(pDX, IDC_PROGRESS4, m_Column4);
	DDX_Control(pDX, IDC_PROGRESS5, m_Column5);
	DDX_Control(pDX, IDC_PROGRESS6, m_Column6);
	DDX_Control(pDX, IDC_PROGRESS7, m_Column7);
	DDX_Control(pDX, IDC_PROGRESS8, m_Column8);
	DDX_Control(pDX, IDC_PROGRESS9, m_Column9);
	DDX_Control(pDX, IDC_PROGRESS10, m_Column10);
	DDX_Control(pDX, IDC_PROGRESS11, m_Column11);
	DDX_Control(pDX, IDC_PROGRESS12, m_Column12);
	DDX_Control(pDX, IDC_PROGRESS13, m_Column13);
	DDX_Control(pDX, IDC_PROGRESS14, m_Column14);
	DDX_Control(pDX, IDC_PROGRESS15, m_Column15);
	DDX_Control(pDX, IDC_PROGRESS16, m_Column16);
	DDX_Control(pDX, IDC_PROGRESS17, m_Column17);
	DDX_Control(pDX, IDC_PROGRESS18, m_Column18);
	DDX_Control(pDX, IDC_PROGRESS19, m_Column19);
	DDX_Control(pDX, IDC_PROGRESS20, m_Column20);
	DDX_Control(pDX, IDC_PROGRESS21, m_Column21);
	DDX_Control(pDX, IDC_PROGRESS22, m_Column22);
	DDX_Control(pDX, IDC_PROGRESS23, m_Column23);
	DDX_Control(pDX, IDC_PROGRESS24, m_Column24);
	DDX_Control(pDX, IDC_PROGRESS25, m_Column25);
	DDX_Control(pDX, IDC_PROGRESS26, m_Column26);
	DDX_Control(pDX, IDC_PROGRESS27, m_Column27);
	DDX_Control(pDX, IDC_PROGRESS28, m_Column28);
	DDX_Control(pDX, IDC_PROGRESS29, m_Column29);
	DDX_Control(pDX, IDC_PROGRESS30, m_Column30);
	DDX_Control(pDX, IDC_PROGRESS31, m_Column31);
	DDX_Control(pDX, IDC_PROGRESS32, m_Column32);
	DDX_Control(pDX, IDC_RADIO1, m_radioPos1);
	DDX_Control(pDX, IDC_RADIO2, m_radioPos2);
	DDX_Control(pDX, IDC_RADIO3, m_radioPos3);
	DDX_Control(pDX, IDC_RADIO4, m_radioPos4);
	DDX_Control(pDX, IDC_RADIO5, m_radioPos5);
	DDX_Control(pDX, IDC_EDIT_START_ISSUE, m_editStartIssue);
	DDX_Control(pDX, IDC_EDIT_END_ISSUE, m_editEndIssue);
}


BEGIN_MESSAGE_MAP(CStepsTab, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE, &CStepsTab::OnBnClickedButtonUpdate)
	ON_BN_CLICKED(IDC_RADIO1, &CStepsTab::OnBnClickedRadio1)
	ON_BN_CLICKED(IDC_RADIO2, &CStepsTab::OnBnClickedRadio2)
	ON_BN_CLICKED(IDC_RADIO3, &CStepsTab::OnBnClickedRadio3)
	ON_BN_CLICKED(IDC_RADIO4, &CStepsTab::OnBnClickedRadio4)
	ON_BN_CLICKED(IDC_RADIO5, &CStepsTab::OnBnClickedRadio5)
END_MESSAGE_MAP()


// CStepsTab message handlers
BOOL CStepsTab::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	if (!m_ToolTipCtrl.Create(this, TTS_ALWAYSTIP))
	{
		TRACE(_T("Unable To create ToolTip\n"));
		return FALSE;
	}

	CString strTemp;
	const LuckyNums& nums = m_pHistory->GetHistory();
	int iStartIssue = nums.begin()->first;
	strTemp.Format(_T("%d"), iStartIssue);
	m_editStartIssue.SetWindowText(strTemp);

	int iEndIssue = nums.rbegin()->first;
	strTemp.Format(_T("%d"), iEndIssue);
	m_editEndIssue.SetWindowText(strTemp);

	SetCheck(1);
	Update();

	return TRUE;
}

void CStepsTab::Clear()
{
	for (int i = 0; i < 5; i ++)
		for (int j = 0; j < 32; j ++)
			m_HitMatrix[i][j] = 0;

	m_iMaxHitCount = 0;
}

void CStepsTab::SetCheck(int iPos)
{
	if (m_iActivateBtn == iPos) return;

	m_radioPos1.SetCheck(iPos == 1 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos2.SetCheck(iPos == 2 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos3.SetCheck(iPos == 3 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos4.SetCheck(iPos == 4 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos5.SetCheck(iPos == 5 ? BST_CHECKED : BST_UNCHECKED);
	m_iActivateBtn = iPos;
}

void CStepsTab::Update()
{
	CString strTemp;
	m_editStartIssue.GetWindowText(strTemp);
	int iStartIssue = _ttoi(strTemp);
	m_editEndIssue.GetWindowText(strTemp);
	int iEndIssue = _ttoi(strTemp);

	// Initialize the states;
	Clear();
	const LuckyNums& nums = m_pHistory->GetHistory();
	int iStep = 0;
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		if (it->first >= iStartIssue && it->first <= iEndIssue)
		{
			for (int i = 0; i < 5; i ++)
			{
				iStep = it->second->m_red[i + 1] - it->second->m_red[i];
				m_HitMatrix[i][iStep - 1]++;
			}
		}
	}

	for (int i = 0; i < 5; i ++)
		for (int j = 0; j < 32; j ++)
			if (m_HitMatrix[i][j] > m_iMaxHitCount)
				m_iMaxHitCount = m_HitMatrix[i][j];

	// Set control status.
	UpdateColumns();
}

void CStepsTab::OnBnClickedButtonUpdate()
{
	Update();
}

void CStepsTab::UpdateColumns()
{
	// Update tool tip for each column;
	CProgressCtrl* control[32];
	control[0] = &m_Column1;
	control[1] = &m_Column2;
	control[2] = &m_Column3;
	control[3] = &m_Column4;
	control[4] = &m_Column5;
	control[5] = &m_Column6;
	control[6] = &m_Column7;
	control[7] = &m_Column8;
	control[8] = &m_Column9;
	control[9] = &m_Column10;
	control[10] = &m_Column11;
	control[11] = &m_Column12;
	control[12] = &m_Column13;
	control[13] = &m_Column14;
	control[14] = &m_Column15;
	control[15] = &m_Column16;
	control[16] = &m_Column17;
	control[17] = &m_Column18;
	control[18] = &m_Column19;
	control[19] = &m_Column20;
	control[20] = &m_Column21;
	control[21] = &m_Column22;
	control[22] = &m_Column23;
	control[23] = &m_Column24;
	control[24] = &m_Column25;
	control[25] = &m_Column26;
	control[26] = &m_Column27;
	control[27] = &m_Column28;
	control[28] = &m_Column29;
	control[29] = &m_Column30;
	control[30] = &m_Column31;
	control[31] = &m_Column32;

	CString strTip;
	int iHit = 0, iPos = 0;
	int iMax = max(m_iMaxHitCount, 100);
	for (int i = 0; i < 32; i ++)
	{
		iHit = m_HitMatrix[m_iActivateBtn - 1][i];
		iPos = iHit * 100 / iMax;
		control[i]->SetPos(iPos);

		// Update tool tip.
		strTip.Format(_T("%d"), iHit);
		m_ToolTipCtrl.AddTool(control[i], strTip);
	}
}

BOOL CStepsTab::PreTranslateMessage(MSG* pMsg)
{
	if(pMsg->message== WM_LBUTTONDOWN ||pMsg->message== WM_LBUTTONUP|| pMsg->message== WM_MOUSEMOVE)
		m_ToolTipCtrl.RelayEvent(pMsg);

	return CDialog::PreTranslateMessage(pMsg);
}

void CStepsTab::OnBnClickedRadio1()
{
	SetCheck(1);
	UpdateColumns();
}

void CStepsTab::OnBnClickedRadio2()
{
	SetCheck(2);
	UpdateColumns();
}

void CStepsTab::OnBnClickedRadio3()
{
	SetCheck(3);
	UpdateColumns();
}

void CStepsTab::OnBnClickedRadio4()
{
	SetCheck(4);
	UpdateColumns();
}

void CStepsTab::OnBnClickedRadio5()
{
	SetCheck(5);
	UpdateColumns();
}
