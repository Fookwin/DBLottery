// ContinuityTab.cpp : implementation file
//

#include "stdafx.h"
#include "..\LuckyBalls.h"
#include "SimilarityTab.h"
#include "..\Server\Global.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Utilities\AnalysisUtil.h"
#include <fstream>
#include <iostream>

// CContinuityTab dialog
IMPLEMENT_DYNAMIC(CSimilarityTab, CDialog)

CSimilarityTab::CSimilarityTab(CWnd* pParent /*=NULL*/)
	: CDialog(CSimilarityTab::IDD, pParent), m_pHistory(NULL)
{

}

CSimilarityTab::~CSimilarityTab()
{
}

void CSimilarityTab::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_PROGRESS_0, m_Column0);
	DDX_Control(pDX, IDC_PROGRESS_1, m_Column1);
	DDX_Control(pDX, IDC_PROGRESS_2, m_Column2);
	DDX_Control(pDX, IDC_PROGRESS_3, m_Column3);
	DDX_Control(pDX, IDC_PROGRESS_4, m_Column4);
	DDX_Control(pDX, IDC_PROGRESS_5, m_Column5);
	DDX_Control(pDX, IDC_PROGRESS_6, m_Column6);
	DDX_Control(pDX, IDC_STATIC_STATE0, m_State0);
	DDX_Control(pDX, IDC_STATIC_STATE1, m_State1);
	DDX_Control(pDX, IDC_STATIC_STATE2, m_State2);
	DDX_Control(pDX, IDC_STATIC_STATE3, m_State3);
	DDX_Control(pDX, IDC_STATIC_STATE4, m_State4);
	DDX_Control(pDX, IDC_STATIC_STATE5, m_State5);
	DDX_Control(pDX, IDC_STATIC_STATE6, m_State6);
	DDX_Control(pDX, IDC_EDIT_START_ISSUE, m_editStartIssue);
	DDX_Control(pDX, IDC_EDIT_END_ISSUE, m_editEndIssue);
	DDX_Control(pDX, IDC_COMBO_ISSUE, m_comboxTestIssue);
	DDX_Control(pDX, IDC_CHECK_SPECIFY_FILE, m_checkSpecifyFile);
}


BEGIN_MESSAGE_MAP(CSimilarityTab, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE, &CSimilarityTab::OnBnClickedButtonUpdate)
	ON_EN_CHANGE(IDC_EDIT_NUM, &CSimilarityTab::OnEnChangeEditNum)
	ON_CBN_SELCHANGE(IDC_COMBO_ISSUE, &CSimilarityTab::OnCbnSelchangeComboIssue)
	ON_BN_CLICKED(IDC_BUTTON_UPDATE2, &CSimilarityTab::OnBnClickedButtonUpdate2)
END_MESSAGE_MAP()


// CSimilarityTab message handlers
BOOL CSimilarityTab::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	m_Column0.SetRange(0, 1000);
	m_Column1.SetRange(0, 1000);
	m_Column2.SetRange(0, 1000);
	m_Column3.SetRange(0, 1000);
	m_Column4.SetRange(0, 1000);
	m_Column5.SetRange(0, 1000);
	m_Column6.SetRange(0, 1000);

	CString strTemp;
	for (int i = 1; i <= 6; i ++)
	{
		strTemp.Format(_T("%d"), i);
		m_comboxTestIssue.AddString(strTemp);
	}
	m_comboxTestIssue.SetCurSel(0);

	m_checkSpecifyFile.SetCheck(TRUE);

	const LuckyNums& nums = m_pHistory->GetHistory();
	int iStartIssue = nums.begin()->first;
	strTemp.Format(_T("%d"), iStartIssue);
	m_editStartIssue.SetWindowText(strTemp);

	int iEndIssue = nums.rbegin()->first;
	strTemp.Format(_T("%d"), iEndIssue);
	m_editEndIssue.SetWindowText(strTemp);

	return TRUE;
}

bool CSimilarityTab::ReadRecord(CString strFile, int iTestCount, NumStatusVec& result, int& lastIssue)
{
	CNumStatus* pData = NULL;

	lastIssue = -1;
	int iIndex = 0;
	int num=0;
	std::ifstream in(strFile,std::ios::out);
	in>>lastIssue;

	while(in.good())
	{
		++iIndex;

		in>>num;

		if (iIndex == 1)
		{
			pData = new CNumStatus();
			pData->m_Num.m_red[0] = num;
		}
		else if (iIndex > 1 && iIndex <= iTestCount)
		{
			pData->m_Num.m_red[iIndex - 1] = num;
		}
		else if (iIndex > iTestCount && iIndex < iTestCount * 2)
		{
			pData->m_Status[iIndex - iTestCount - 1] = num;
		}
		else if (iIndex == iTestCount * 2)
		{
			pData->m_Status[iIndex - iTestCount - 1] = num;

			result.push_back(pData);
			pData = NULL;
			iIndex = 0;
		}
	}

	in.close();

	return true;
}

bool CSimilarityTab::SaveRecord(CString strFile, int iTestCount, NumStatusVec& result, int lastIssue)
{
	CString strTemp;
	std::ofstream out(strFile,std::ios::trunc);

	strTemp.Format(_T("%d\n"), lastIssue);
	out<<CW2A(strTemp);

	for (NumStatusVec::iterator it = result.begin(); it != result.end(); it ++)
	{
		for (int i = 0; i < iTestCount; i ++)
		{
			strTemp.Format(_T("%.2d"), (*it)->m_Num.m_red[i]);
			out<<CW2A(strTemp);
			if (i < 5)
				out<<" ";
		}

		for (int i = 0; i < iTestCount; i ++)
		{
			out<<"\t";
			strTemp.Format(_T("%d"), (*it)->m_Status[i]);
			out<<CW2A(strTemp);
		}
		out<<"\n";
	}

	out.close();

	return true;
}

void CSimilarityTab::Update2()
{
	int iTestValue = m_comboxTestIssue.GetCurSel() + 1;
	CString pathName;
	if (m_checkSpecifyFile.GetCheck())
	{
		// "*.my" for "MyType Files" and "*.*' for "All Files."
		CString strFileType = _T("Text Files (*.txt)|*.txt|All Files (*.*)|*.*||");

		// Create an Open dialog; the default file name extension is ".my".
		CFileDialog fileDlg(TRUE, _T("txt"), _T("*.txt"), OFN_HIDEREADONLY, strFileType, this);

		// Display the file dialog. When user clicks OK, fileDlg.DoModal() 
		// returns IDOK.
		if( fileDlg.DoModal()!=IDOK )
			return;

		pathName = fileDlg.GetPathName();
	}
	else
	{
		pathName = _T("C:\\Users\\zhangze\\Desktop\\Lotto\\");
		CString strVal; strVal.Format(_T("Coverage_%d.txt"), iTestValue);
		pathName += strVal;
	}

	// Get previous result
	bool bNewFile = false;
	NumStatusVec result;
	int ilastIssue = 0;
	if (::PathFileExists(pathName))
	{
		if (!ReadRecord(pathName, iTestValue, result, ilastIssue))
			return;

		// validating...
		int iCorrectSize = 0;
		switch (iTestValue)
		{
		case 1: iCorrectSize = 33; break;
		case 2: iCorrectSize = 528; break;
		case 3: iCorrectSize = 5456; break;
		case 4: iCorrectSize = 40920; break;
		case 5: iCorrectSize = 237336; break;
		case 6: iCorrectSize = 1107568; break;
		}

		if (result.size() != iCorrectSize)
		{
			// clean up.
			for (NumStatusVec::const_iterator it = result.begin(); it != result.end(); it ++)
			{
				delete *it;
			}
			result.clear();

			int iRes = ::MessageBox(this->GetSafeHwnd(), _T("Failed to parse the correct data from the specified file. \nDo you want to recompute and override it?"), _T("Error!"), MB_YESNO);
			if (iRes == IDYES)
			{
				// Construct
				InitNumStatus(result, iTestValue);
				ilastIssue = 0;
			}
			else
				return;
		}
	}
	else
	{
		// Construct
		InitNumStatus(result, iTestValue);

		bNewFile = true;
	}

	bool bAnyUpdate = false;

	const LuckyNums& nums = m_pHistory->GetHistory();

	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++)
	{
		if (it->first <= ilastIssue)
			continue;

		for (NumStatusVec::iterator itRlt = result.begin(); itRlt != result.end(); itRlt ++)
		{
			int iSimulity = CAnalysisUtil::GetSimilarity((*itRlt)->m_Num, *it->second);
			if (iSimulity > 0)
			{
				(*itRlt)->m_Status[iSimulity - 1] ++;
				bAnyUpdate = true;
			}
		}

		ilastIssue = it->first;
	}

	if (bAnyUpdate)
	{
		if (bNewFile)
		{
			// Create the file.
			CFile file(pathName, CFile::modeCreate | CFile::modeWrite);
			file.Close();
		}
		// Save result.
		SaveRecord(pathName, iTestValue, result, ilastIssue);
	}

	// clean up.
	for (NumStatusVec::const_iterator it = result.begin(); it != result.end(); it ++)
	{
		delete *it;
	}
	result.clear();

	int iRes = ::MessageBox(this->GetSafeHwnd(), _T("Do you want to open the file location?"), _T("Done"), MB_YESNO);
	if (iRes == IDYES)
	{
		// Get the location of the file.
		CString strTemp = pathName;
		strTemp.Replace('//', '\\');
		int iPos = strTemp.ReverseFind('\\');
		CString strLocation = strTemp.Left(iPos + 1);

		STARTUPINFO si = {sizeof(si)}; 
		PROCESS_INFORMATION pi;
		si.dwFlags = STARTF_USESHOWWINDOW; 
		si.wShowWindow = TRUE; 
		CString cmd = _T("explorer.EXE ");
		cmd += strLocation;
		BOOL ret = ::CreateProcess(NULL, cmd.GetBuffer(), NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi);
	}
}

void CSimilarityTab::Update()
{
	// Start progress...
	Lucky::InitProgress();
	Lucky::ShowProgress(true);
	Lucky::SetProgress(_T("Calculating similarity ..."));

	CString strTemp;
	m_editStartIssue.GetWindowText(strTemp);
	int iStartIssue = _ttoi(strTemp);
	m_editEndIssue.GetWindowText(strTemp);
	int iEndIssue = _ttoi(strTemp);


	const LuckyNums& nums = m_pHistory->GetHistory();
	int iHit[7] = {0};
	int iCount = (int)nums.size(), iIndex = 0;
	for (LuckyNums::const_iterator it = nums.begin(); it != nums.end(); it ++, iIndex ++)
	{
		// update task...
		CString strTask;
		strTask.Format(_T("analyzing %d, %d, %d, %d, %d, %d ..."),
			it->second->m_red[0], it->second->m_red[1], it->second->m_red[2],
			it->second->m_red[3], it->second->m_red[4], it->second->m_red[5]);
		Lucky::SetTask(strTask);

		if (it->first >= iStartIssue && it->first <= iEndIssue)
		{
			CSimilarity st;
			CalSimilarity(*(it->second), m_pHistory->GetHistory(), st, iStartIssue, iEndIssue, it->first);
			iHit[st.m_iMax] ++;

			if (st.m_iMax < 4)
			{
				CString strMsg; strMsg.Format(_T("%d, %d, %d, %d, %d, %d"), it->second->m_red[0], it->second->m_red[1], it->second->m_red[2],
					it->second->m_red[3], it->second->m_red[4], it->second->m_red[5]);
				::MessageBox(NULL, strMsg, _T("Max hits for the follow numbers less than 4 times!"), MB_OK);
			}
		}

		// Update progress...
		Lucky::SetPos(iIndex * 1000 / iCount);
	}

	Lucky::ShowProgress(false);

	// Set control status.
	m_Column0.SetPos(iHit[0] * 1000 / iCount);
	m_Column1.SetPos(iHit[1] * 1000 / iCount);
	m_Column2.SetPos(iHit[2] * 1000 / iCount);
	m_Column3.SetPos(iHit[3] * 1000 / iCount);
	m_Column4.SetPos(iHit[4] * 1000 / iCount);
	m_Column5.SetPos(iHit[5] * 1000 / iCount);
	m_Column6.SetPos(iHit[6] * 1000 / iCount);

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
	strTemp.Format(_T("%d"), iHit[6]);
	m_State6.SetWindowText(strTemp);
}

void CSimilarityTab::OnBnClickedButtonUpdate()
{
	Update2();	
}

void CSimilarityTab::OnEnChangeEditNum()
{
	// TODO:  If this is a RICHEDIT control, the control will not
	// send this notification unless you override the CDialog::OnInitDialog()
	// function and call CRichEditCtrl().SetEventMask()
	// with the ENM_CHANGE flag ORed into the mask.

	// TODO:  Add your control notification handler code here
}

void CSimilarityTab::OnCbnSelchangeComboIssue()
{

}

void CSimilarityTab::CalSimilarity(const CLuckyNum& test, const LuckyNums& history, 
								  CSimilarity& st, int iStartIssue, int iEndIssue,int iExceptIssue)
{
	for (LuckyNums::const_iterator it = history.begin(); it != history.end(); it ++)
	{
		if (iStartIssue >= 0 && it->first < iStartIssue) continue;
		if (iEndIssue >= 0 && it->first > iEndIssue) continue;
		if (iExceptIssue >= 0 && iExceptIssue == it->first) continue;

		int iHit = 0;
		for (int i = 0; i < 6; i ++)
		{
			for (int j = 0; j < 6; j ++)
			{
				if (it->second->m_red[i] == test.m_red[j])
				{
					iHit ++;
					break;
				}
			}
		}

		st.m_HitCounts[iHit] ++;

		if (iHit < st.m_iMin) st.m_iMin = iHit;
		else if (iHit > st.m_iMax) st.m_iMax = iHit;
	}
}

void CSimilarityTab::OnBnClickedButtonUpdate2()
{
	Update();
}

void CSimilarityTab::InitNumStatus(NumStatusVec& result, int iTestCount /*= 6*/)
{
	if (iTestCount <= 0 || iTestCount > 6)
		return;
	
	CNumStatus* pData = NULL;

	// Get all numbers.
	int iIndex = 0;
	for (int inx1 = 1; inx1 <= 34 - iTestCount; inx1 ++)
	{
		if (iTestCount < 2)
		{
			pData = new CNumStatus();
			result.push_back(pData);

			pData->m_Num.m_red[0] = inx1;
			continue;
		}

		for (int inx2 = inx1 + 1; inx2 <= 35 - iTestCount; inx2 ++)
		{			
			if (iTestCount < 3)
			{
				pData = new CNumStatus();
				result.push_back(pData);

				pData->m_Num.m_red[0] = inx1;
				pData->m_Num.m_red[1] = inx2;
				
				continue;
			}

			for (int inx3 = inx2 + 1; inx3 <= 36 - iTestCount; inx3 ++)
			{				
				if (iTestCount < 4)
				{
					pData = new CNumStatus();
					result.push_back(pData);

					pData->m_Num.m_red[0] = inx1;
					pData->m_Num.m_red[1] = inx2;
					pData->m_Num.m_red[2] = inx3;

					continue;
				}

				for (int inx4 = inx3 + 1; inx4 <= 37 - iTestCount; inx4 ++)
				{					
					if (iTestCount < 5)
					{
						pData = new CNumStatus();
						result.push_back(pData);

						pData->m_Num.m_red[0] = inx1;
						pData->m_Num.m_red[1] = inx2;
						pData->m_Num.m_red[2] = inx3;
						pData->m_Num.m_red[3] = inx4;
						continue;
					}

					for (int inx5 = inx4 + 1; inx5 <= 38 - iTestCount; inx5 ++)
					{						
						if (iTestCount < 6)
						{
							pData = new CNumStatus();
							result.push_back(pData);

							pData->m_Num.m_red[0] = inx1;
							pData->m_Num.m_red[1] = inx2;
							pData->m_Num.m_red[2] = inx3;
							pData->m_Num.m_red[3] = inx4;
							pData->m_Num.m_red[4] = inx5;
							continue;
						}

						for (int inx6 = inx5 + 1; inx6 <= 39 - iTestCount; inx6 ++)
						{
							pData = new CNumStatus();
							result.push_back(pData);

							pData->m_Num.m_red[0] = inx1;
							pData->m_Num.m_red[1] = inx2;
							pData->m_Num.m_red[2] = inx3;
							pData->m_Num.m_red[3] = inx4;
							pData->m_Num.m_red[4] = inx5;
							pData->m_Num.m_red[5] = inx6;
						}
					}
				}
			}
		}
	}
}

///////////////////////////////////////////////////////////////////
CSimilarityTab::CNumStatus::CNumStatus()
{
	for (int i = 0; i <= 5; ++ i)
		m_Status[i] = 0;
}