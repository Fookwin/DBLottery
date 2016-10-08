// HitStatisticsTab.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "HitStatisticsTab.h"
#include "..\Data\Analysis\HistoryData.h"


// CHitStatisticsTab dialog

IMPLEMENT_DYNAMIC(CHitStatisticsTab, CDialog)

CHitStatisticsTab::CHitStatisticsTab(CWnd* pParent /*=NULL*/)
	: CDialog(CHitStatisticsTab::IDD, pParent), m_pHistory(NULL), m_iActivateBtn(0), m_iMaxHitCount(0)
{
	Clear();
}

CHitStatisticsTab::~CHitStatisticsTab()
{
}

void CHitStatisticsTab::DoDataExchange(CDataExchange* pDX)
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
	DDX_Control(pDX, IDC_PROGRESS33, m_Column33);
	DDX_Control(pDX, IDC_PROGRESS33, m_Column33);
	DDX_Control(pDX, IDC_RADIO1, m_radioPos1);
	DDX_Control(pDX, IDC_RADIO2, m_radioPos2);
	DDX_Control(pDX, IDC_RADIO3, m_radioPos3);
	DDX_Control(pDX, IDC_RADIO4, m_radioPos4);
	DDX_Control(pDX, IDC_RADIO5, m_radioPos5);
	DDX_Control(pDX, IDC_RADIO6, m_radioPos6);
	DDX_Control(pDX, IDC_EDIT_START_ISSUE, m_editStartIssue);
	DDX_Control(pDX, IDC_EDIT_END_ISSUE, m_editEndIssue);
}


BEGIN_MESSAGE_MAP(CHitStatisticsTab, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE, &CHitStatisticsTab::OnBnClickedButtonUpdate)
	ON_BN_CLICKED(IDC_RADIO1, &CHitStatisticsTab::OnBnClickedRadio1)
	ON_BN_CLICKED(IDC_RADIO2, &CHitStatisticsTab::OnBnClickedRadio2)
	ON_BN_CLICKED(IDC_RADIO3, &CHitStatisticsTab::OnBnClickedRadio3)
	ON_BN_CLICKED(IDC_RADIO4, &CHitStatisticsTab::OnBnClickedRadio4)
	ON_BN_CLICKED(IDC_RADIO5, &CHitStatisticsTab::OnBnClickedRadio5)
	ON_BN_CLICKED(IDC_RADIO6, &CHitStatisticsTab::OnBnClickedRadio6)
END_MESSAGE_MAP()


// CHitStatisticsTab message handlers
BOOL CHitStatisticsTab::OnInitDialog()
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

void CHitStatisticsTab::Clear()
{
	for (int i = 0; i < 6; i ++)
		for (int j = 0; j < 33; j ++)
			m_HitMatrix[i][j] = 0;

	m_iMaxHitCount = 0;
}

void CHitStatisticsTab::Update()
{
	CString strTemp;
	m_editStartIssue.GetWindowText(strTemp);
	int iStartIssue = _ttoi(strTemp);
	m_editEndIssue.GetWindowText(strTemp);
	int iEndIssue = _ttoi(strTemp);

	// Initialize the states;
	Clear();

	const LuckyNums& nums = m_pHistory->GetHistory();
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		if (it->first >= iStartIssue && it->first <= iEndIssue)
		{
			m_HitMatrix[0][it->second->m_red[0] - 1]++;
			m_HitMatrix[1][it->second->m_red[1] - 1]++;
			m_HitMatrix[2][it->second->m_red[2] - 1]++;
			m_HitMatrix[3][it->second->m_red[3] - 1]++;
			m_HitMatrix[4][it->second->m_red[4] - 1]++;
			m_HitMatrix[5][it->second->m_red[5] - 1]++;
		}
	}

	// Set control status.
	for (int i = 0; i < 6; i ++)
		for (int j = 0; j < 33; j ++)
			if (m_HitMatrix[i][j] > m_iMaxHitCount)
				m_iMaxHitCount = m_HitMatrix[i][j];

	UpdateColumns();
}

void CHitStatisticsTab::SetCheck(int iPos)
{
	if (m_iActivateBtn == iPos) return;

	m_radioPos1.SetCheck(iPos == 1 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos2.SetCheck(iPos == 2 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos3.SetCheck(iPos == 3 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos4.SetCheck(iPos == 4 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos5.SetCheck(iPos == 5 ? BST_CHECKED : BST_UNCHECKED);
	m_radioPos6.SetCheck(iPos == 6 ? BST_CHECKED : BST_UNCHECKED);
	m_iActivateBtn = iPos;
}

void CHitStatisticsTab::UpdateColumns()
{
	// Update tool tip for each column;
	CProgressCtrl* control[33];
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
	control[32] = &m_Column33;

	CString strTip;
	int iHit = 0;
	int iPos = 0;
	int iMax = max(m_iMaxHitCount, 100);
	for (int i = 0; i < 33; i ++)
	{
		iHit = m_HitMatrix[m_iActivateBtn - 1][i];
		iPos = iHit * 100 / iMax;
		control[i]->SetPos(iPos);

		// Update tool tip.
		strTip.Format(_T("%d"), iHit);
		m_ToolTipCtrl.AddTool(control[i], strTip);
	}
}

BOOL CHitStatisticsTab::PreTranslateMessage(MSG* pMsg)
{
	if(pMsg->message== WM_LBUTTONDOWN ||pMsg->message== WM_LBUTTONUP|| pMsg->message== WM_MOUSEMOVE)
		m_ToolTipCtrl.RelayEvent(pMsg);

	return CDialog::PreTranslateMessage(pMsg);
}

void CHitStatisticsTab::OnBnClickedButtonUpdate()
{
	Update();
}

void CHitStatisticsTab::OnBnClickedRadio1()
{
	SetCheck(1);
	UpdateColumns();
}

void CHitStatisticsTab::OnBnClickedRadio2()
{
	SetCheck(2);
	UpdateColumns();
}

void CHitStatisticsTab::OnBnClickedRadio3()
{
	SetCheck(3);
	UpdateColumns();
}

void CHitStatisticsTab::OnBnClickedRadio4()
{
	SetCheck(4);
	UpdateColumns();
}

void CHitStatisticsTab::OnBnClickedRadio5()
{
	SetCheck(5);
	UpdateColumns();
}

void CHitStatisticsTab::OnBnClickedRadio6()
{
	SetCheck(6);
	UpdateColumns();
}
