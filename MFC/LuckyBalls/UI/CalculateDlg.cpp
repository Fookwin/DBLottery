// HistoryDialog.cpp : implementation file
//

#include "stdafx.h"
#include "..\Data\Analysis\HistoryData.h"
#include "..\Data\Constraint\NumberSetConstraint.h"
#include "CalculateDlg.h"
#include "StratgiesTestDlg.h"
#include "Controls\SelectNumDlg.h"
#include "..\Utilities\ConstraintUtil.h"
#include "Controls\NumberRestrictsDlg.h"
#include <fstream>
#include <iostream>
#include "..\Server\Global.h"

IMPLEMENT_DYNAMIC(CNumSetEdit, CEdit)

CNumSetEdit::CNumSetEdit(const CRagion& ragion, int iDispSpace)
: CEdit(), m_numRagion(ragion), m_iSpace(iDispSpace)
{
}

CNumSetEdit::~CNumSetEdit()
{
}

BEGIN_MESSAGE_MAP(CNumSetEdit, CEdit)
	ON_WM_RBUTTONDOWN()
END_MESSAGE_MAP()

CNumSet CNumSetEdit::GetNumSet() const
{
	CString str;
	GetWindowText(str);
	return CNumSet(str);
}

void CNumSetEdit::OnRButtonDown(UINT nFlags, CPoint point)
{
	if (!::IsWindow(this->GetSafeHwnd()))
		return;

	CString str;
	GetWindowText(str);
	CNumSet numSet;
	if (numSet.SetText(str))
	{
		int iSpace = 1;
		CSelectNumDlg select(numSet, m_numRagion, m_iSpace, _T("Select Numbers"), this);
		if (select.DoModal() == IDOK)
		{
			select.GetNumSet(numSet);
			SetWindowText(numSet.GetText());
		}
	}
}

// CCalculateDlg dialog

IMPLEMENT_DYNAMIC(CCalculateDlg, CDialog)

CCalculateDlg::CCalculateDlg(CWnd* pParent /*=NULL*/)
:	CDialog(CCalculateDlg::IDD, pParent),
	m_pHistory(NULL),
	m_editStep12(CRagion(1,27)),
	m_editStep23(CRagion(1,27)),
	m_editStep34(CRagion(1,27)),
	m_editStep45(CRagion(1,27)),
	m_editStep56(CRagion(1,27)),
	m_editContinuity(CRagion(0,5)),
	m_editEven(CRagion(0,6)),
	m_editPrime(CRagion(0,6)),
	m_editRepeat(CRagion(0,6)),
	m_editSmall(CRagion(0,6)),
	m_editSum(CRagion(21,162), 10),
	m_editMissing(CRagion(0,60), 5)
{
}

CCalculateDlg::~CCalculateDlg()
{
	CAnalysisUtil::DeleteAll(m_CalResult);
}

void CCalculateDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_EDIT_RED1, m_editRed1);
	DDX_Control(pDX, IDC_EDIT_RED2, m_editRed2);
	DDX_Control(pDX, IDC_EDIT_RED3, m_editRed3);
	DDX_Control(pDX, IDC_EDIT_RED4, m_editRed4);
	DDX_Control(pDX, IDC_EDIT_RED5, m_editRed5);
	DDX_Control(pDX, IDC_EDIT_RED6, m_editRed6);
	DDX_Control(pDX, IDC_EDIT_STEP12, m_editStep12);
	DDX_Control(pDX, IDC_EDIT_STEP23, m_editStep23);
	DDX_Control(pDX, IDC_EDIT_STEP34, m_editStep34);
	DDX_Control(pDX, IDC_EDIT_STEP45, m_editStep45);
	DDX_Control(pDX, IDC_EDIT_STEP56, m_editStep56);
	DDX_Control(pDX, IDC_EDIT_MISSING, m_editMissing);
	DDX_Control(pDX, IDC_EDIT_CONTINUITY, m_editContinuity);
	DDX_Control(pDX, IDC_LIST_RESULT, m_ResultList);
	DDX_Control(pDX, IDC_CHECK_FIND_IN_RESULT, m_btnFindInResult);
	DDX_Control(pDX, IDC_EDIT_INCLUDE, m_editRedRestricts);
	DDX_Control(pDX, IDC_EDIT_EVEN, m_editEven);
	DDX_Control(pDX, IDC_EDIT_PRIME_COUNT, m_editPrime);
	DDX_Control(pDX, IDC_EDIT_REPEAT, m_editRepeat);
	DDX_Control(pDX, IDC_EDIT_SMALL_COUNT, m_editSmall);
	DDX_Control(pDX, IDC_EDIT_SUM, m_editSum);
	DDX_Control(pDX, IDC_CHECK_RED_RAGION, m_btnCheckRed);
	DDX_Control(pDX, IDC_CHECK_INCLUDE, m_btnCheckRedGeneral);
	DDX_Control(pDX, IDC_CHECK_IMMEDIATE_SIMULARITY2, m_btnCheckRepeat);
	DDX_Control(pDX, IDC_CHECK_MAX_CONTINUITY, m_btnCheckContinuity);
	DDX_Control(pDX, IDC_CHECK_EVEN_COUNT, m_btnCheckEvenCount);
	DDX_Control(pDX, IDC_CHECK_SUM, m_btnCheckSum);
	DDX_Control(pDX, IDC_CHECK_RED_STEPS, m_btnCheckRedStep);
	DDX_Control(pDX, IDC_CHECK_PRIME, m_btnCheckPrime);
	DDX_Control(pDX, IDC_CHECK_SMALL_COUNT, m_btnCheckSmall);
	DDX_Control(pDX, IDC_CHECK_MISSING, m_btnCheckMissing);
	DDX_Control(pDX, IDC_RADIO_645, m_btnRadio645);
	DDX_Control(pDX, IDC_RADIO_644, m_btnRadio644);
	DDX_Control(pDX, IDC_EDIT_RED_SELECTION, m_editRedSelection);
	DDX_Control(pDX, IDC_COMBO_TEST_ISSUES, m_TestRange);
	DDX_Control(pDX, IDC_CHECK_EVALUATE_RESULT, m_checkEvaluateResult);
}


BEGIN_MESSAGE_MAP(CCalculateDlg, CDialog)
	ON_BN_CLICKED(IDC_BUTTON_SAVE_RESULT, &CCalculateDlg::OnBnClickedButtonSaveResult)
	ON_BN_CLICKED(IDC_BUTTON_LOAD_RESULT, &CCalculateDlg::OnBnClickedButtonLoadResult)
	ON_BN_CLICKED(ID_BUTTON_SUGGEST, &CCalculateDlg::OnBnClickedButtonSuggest)
	ON_NOTIFY(LVN_GETDISPINFO, IDC_LIST_RESULT, &CCalculateDlg::OnLvnGetdispinfoListResult)
	ON_BN_CLICKED(IDC_BUTTON_AS_DEFAULT, &CCalculateDlg::OnBnClickedButtonAsDefault)
	ON_BN_CLICKED(IDC_BUTTON_IMPORT, &CCalculateDlg::OnBnClickedButtonImport)
	ON_BN_CLICKED(IDC_BUTTON_EXPORT, &CCalculateDlg::OnBnClickedButtonExport)
	ON_BN_CLICKED(IDC_BUTTON_EDIT_RED, &CCalculateDlg::OnBnClickedButtonEditRed)
	ON_BN_CLICKED(IDC_CHECK_MATRIX, &CCalculateDlg::OnBnClickedCheckMatrix)
	ON_BN_CLICKED(IDC_RADIO_645, &CCalculateDlg::OnBnClickedRadio645)
	ON_BN_CLICKED(IDC_RADIO_644, &CCalculateDlg::OnBnClickedRadio545)
	ON_BN_CLICKED(IDC_BUTTON_MATRIX_FILTER, &CCalculateDlg::OnBnClickedButtonMatrixFilter)
	ON_BN_CLICKED(IDC_BUTTON_STRATGIES_TEST, &CCalculateDlg::OnBnClickedButtonStratgiesTest)
	ON_CBN_SELCHANGE(IDC_COMBO_TEST_ISSUES, &CCalculateDlg::OnCbnSelchangeComboTestIssues)
END_MESSAGE_MAP()


// CCalculateDlg message handlers
BOOL CCalculateDlg::OnInitDialog()
{
	if (!__super::OnInitDialog())
		return FALSE;

	DWORD dwStyle2 = m_ResultList.GetExtendedStyle();
	dwStyle2 |= LVS_EX_FULLROWSELECT;
	dwStyle2 |= LVS_EX_GRIDLINES;
	m_ResultList.SetExtendedStyle(dwStyle2);

	m_ResultList.InsertColumn(0, _T("–Ú∫≈"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(1, _T("∫Ï«Ú1"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(2, _T("∫Ï«Ú2"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(3, _T("∫Ï«Ú3"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(4, _T("∫Ï«Ú4"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(5, _T("∫Ï«Ú5"), LVCFMT_CENTER, 50);
	m_ResultList.InsertColumn(6, _T("∫Ï«Ú6"), LVCFMT_CENTER, 50);

	// Set default Constraints.
	Constraints Constraints;
	Constraints.AsDefault();
	SetConditionCtrls(Constraints);

	m_btnRadio644.SetCheck(BST_CHECKED);
	m_editRedSelection.SetWindowText(_T("1,2,3,4,5,6,7,8"));

	m_TestRange.InsertString(0, _T("Last 5"));
	m_TestRange.InsertString(1, _T("Last 10"));
	m_TestRange.InsertString(2, _T("Last 30"));
	m_TestRange.InsertString(3, _T("Last 50"));
	m_TestRange.InsertString(4, _T("Last 100"));
	m_TestRange.InsertString(5, _T("Last 200"));
	m_TestRange.SetCurSel(0);

	return TRUE;
}

void CCalculateDlg::SetConditionCtrls(const Constraints& condition)
{
	// Numbers...
	RedInPositionConstraint* pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_1));
	if (pNumSetCt != NULL)
	{
		m_editRed1.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_2));
	if (pNumSetCt != NULL)
	{
		m_editRed2.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_3));
	if (pNumSetCt != NULL)
	{
		m_editRed3.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_4));
	if (pNumSetCt != NULL)
	{
		m_editRed4.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_5));
	if (pNumSetCt != NULL)
	{
		m_editRed5.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_6));
	if (pNumSetCt != NULL)
	{
		m_editRed6.SetWindowText(pNumSetCt->GetNumSet().GetText());
	}

	m_btnCheckRed.SetCheck(pNumSetCt != NULL && !pNumSetCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	RedNumbersConstraint* pRedCt = dynamic_cast<RedNumbersConstraint*>(condition.GetConstraint(VALID_RED));
	if (pRedCt != NULL)
	{
		m_editRedRestricts.SetWindowText(pRedCt->GetText());
	}

	m_btnCheckRedGeneral.SetCheck(pRedCt != NULL && !pRedCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	// Steps...
	RedStepConstraint* pNumStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_12));
	if (pNumStepCt != NULL)
	{
		m_editStep12.SetWindowText(pNumStepCt->GetNumSet().GetText());
	}

	pNumStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_23));
	if (pNumStepCt != NULL)
	{
		m_editStep23.SetWindowText(pNumStepCt->GetNumSet().GetText());
	}

	pNumStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_34));
	if (pNumStepCt != NULL)
	{
		m_editStep34.SetWindowText(pNumStepCt->GetNumSet().GetText());
	}

	pNumStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_45));
	if (pNumStepCt != NULL)
	{
		m_editStep45.SetWindowText(pNumStepCt->GetNumSet().GetText());
	}

	pNumStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_56));
	if (pNumStepCt != NULL)
	{
		m_editStep56.SetWindowText(pNumStepCt->GetNumSet().GetText());
	}
	m_btnCheckRedStep.SetCheck(pNumStepCt != NULL && !pNumStepCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	// properties...
	NumberSumConstraint* pSumCt = dynamic_cast<NumberSumConstraint*>(condition.GetConstraint(VALID_SUM));
	if (pSumCt != NULL)
	{
		m_editSum.SetWindowText(pSumCt->GetNumSet().GetText());
	}
	m_btnCheckSum.SetCheck(pSumCt != NULL && !pSumCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	RepeatCountConstraint* pRepeatCt = dynamic_cast<RepeatCountConstraint*>(condition.GetConstraint(VALID_REPEAT_COUNT));
	if (pRepeatCt != NULL)
	{
		m_editRepeat.SetWindowText(pRepeatCt->GetNumSet().GetText());
	}
	m_btnCheckRepeat.SetCheck(pRepeatCt != NULL && !pRepeatCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	EvenCountConstraint* pEvenCt = dynamic_cast<EvenCountConstraint*>(condition.GetConstraint(VALID_EVEN_COUNT));
	if (pEvenCt != NULL)
	{
		m_editEven.SetWindowText(pEvenCt->GetNumSet().GetText());
	}
	m_btnCheckEvenCount.SetCheck(pEvenCt != NULL && !pEvenCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	PrimeNumConstraint* pPrimeCt = dynamic_cast<PrimeNumConstraint*>(condition.GetConstraint(VALID_PRIME_NUM_COUNT));
	if (pPrimeCt != NULL)
	{
		m_editPrime.SetWindowText(pPrimeCt->GetNumSet().GetText());
	}
	m_btnCheckPrime.SetCheck(pPrimeCt != NULL && !pPrimeCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	ContinuityConstraint* pContCt = dynamic_cast<ContinuityConstraint*>(condition.GetConstraint(VALID_CONTINUOUS_COUNT));
	if (pContCt != NULL)
	{
		m_editContinuity.SetWindowText(pContCt->GetNumSet().GetText());
	}
	m_btnCheckContinuity.SetCheck(pContCt != NULL && !pContCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	SmallNumConstraint* pSmallCt = dynamic_cast<SmallNumConstraint*>(condition.GetConstraint(VALID_SMALL_NUM_COUNT));
	if (pSmallCt != NULL)
	{
		m_editSmall.SetWindowText(pSmallCt->GetNumSet().GetText());
	}
	m_btnCheckSmall.SetCheck(pSmallCt != NULL && !pSmallCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);

	TotalMissingConstraint* pMissingCt = dynamic_cast<TotalMissingConstraint*>(condition.GetConstraint(VALID_TOTAL_MISSING));
	if (pMissingCt != NULL)
	{
		m_editMissing.SetWindowText(pMissingCt->GetNumSet().GetText());
	}
	m_btnCheckMissing.SetCheck(pMissingCt != NULL && !pMissingCt->IsSuppressed() ? BST_CHECKED : BST_UNCHECKED);
}

void CCalculateDlg::OnBnClickedButtonLoadResult()
{
	// "*.my" for "MyType Files" and "*.*' for "All Files."
	CString strFileType= _T("LuckyNums Files (*.lns)|*.lns|All Files (*.*)|*.*||");

	// Create an Open dialog; the default file name extension is ".my".
	CFileDialog fileDlg(TRUE, _T("lns"), _T("*.lns"), OFN_FILEMUSTEXIST| OFN_HIDEREADONLY, strFileType, this);
	
	// Display the file dialog. When user clicks OK, fileDlg.DoModal() 
	// returns IDOK.
	if( fileDlg.DoModal()==IDOK )
	{
		CAnalysisUtil::DeleteAll(m_CalResult);

		CString pathName = fileDlg.GetPathName();

		CLuckyNum* pData = NULL;
		int iIndex = 0;
		int num=0;
		std::ifstream in(pathName,std::ios::out);
		while(in.good())
		{
			++iIndex;
			if (iIndex == 7)
			{
				char aSeperate;
				in>>aSeperate;
				continue;
			}

			in>>num;

			if (iIndex == 1)
			{
				pData = new CLuckyNum();
				pData->m_red[0] = num;
			}
			else if (iIndex > 1 && iIndex < 7)
			{
				pData->m_red[iIndex - 1] = num;
			}
			else if (iIndex == 8)
			{
				pData->m_blue = num;
				m_CalResult.push_back(pData);
				pData = NULL;
				iIndex = 0;
			}
		}

		in.close();

		if (pData)
		{
			// delete invalid data.
			delete pData;
			pData = NULL;
		}

		m_ResultList.DeleteAllItems();

		int iSize = (int)m_CalResult.size();
		m_ResultList.SetItemCount(iSize);

		// Load conditions.
		pathName.TrimRight(_T(".lns"));
		pathName += _T(".xml");
		Constraints constraints;
		if (constraints.ReadFrom(pathName))
		{
			this->SetConditionCtrls(constraints);
		}
	}
}

void CCalculateDlg::OnBnClickedButtonSaveResult()
{
	// "*.my" for "MyType Files" and "*.*' for "All Files."
	CString strFileType = _T("LuckyNums Files (*.lns)|*.lns|All Files (*.*)|*.*||");

	// Create an Open dialog; the default file name extension is ".my".
	CFileDialog fileDlg(FALSE, _T("lns"), _T("*.lns"), OFN_FILEMUSTEXIST| OFN_HIDEREADONLY, strFileType, this);
	
	// Display the file dialog. When user clicks OK, fileDlg.DoModal() 
	// returns IDOK.
	if( fileDlg.DoModal()==IDOK )
	{
		CString pathName = fileDlg.GetPathName();

		CFile file(pathName, CFile::modeCreate | CFile::modeWrite);
		file.Close();

		CString strTemp;
		std::ofstream out(pathName,std::ios::in);
		for (CAnalysisUtil::LuckyNumSetIt it = m_CalResult.begin(); it != m_CalResult.end(); it ++)
		{
			for (int i = 0; i < 6; i ++)
			{
				strTemp.Format(_T("%.2d"), (*it)->m_red[i]);
				out<<CW2A(strTemp);
				if (i < 5)
					out<<" ";
			}

			out<<"+";
			strTemp.Format(_T("%.2d"), (*it)->m_blue);
			out<<CW2A(strTemp);
			out<<"\n";
		}

		out.close();

		// Save conditions.
		pathName.TrimRight(_T(".lns"));
		pathName += _T(".xml");
		Constraints constraints;
		if (this->GetConstraints(constraints))
		{
			constraints.WriteTo(pathName);
		}
	}
}

void CCalculateDlg::OnBnClickedButtonSuggest()
{
	Constraints condition;
	if (!GetConstraints(condition))
		return;

	if (m_btnFindInResult.GetCheck() == BST_UNCHECKED)
	{
		// Clear the previous result.
		CAnalysisUtil::DeleteAll(m_CalResult);

		if (!CAnalysisUtil::Suggest(condition, m_CalResult))
			return;
	}
	else
	{
		if (!CAnalysisUtil::SuggestInResult(condition, m_CalResult))
			return;
	}

	m_ResultList.DeleteAllItems();

	int iSize = (int)m_CalResult.size();
	m_ResultList.SetItemCount(iSize);
}

bool CCalculateDlg::GetConstraints(Constraints& condition)
{
	// Check "check buttons"
	bool bCheckRed = m_btnCheckRed.GetCheck() == BST_CHECKED;
	bool bCheckRedGeneral = m_btnCheckRedGeneral.GetCheck() == BST_CHECKED;
	bool bCheckRepeat = m_btnCheckRepeat.GetCheck() == BST_CHECKED;
	bool bCheckConti = m_btnCheckContinuity.GetCheck() == BST_CHECKED;
	bool bCheckEven = m_btnCheckEvenCount.GetCheck() == BST_CHECKED;
	bool bCheckSum = m_btnCheckSum.GetCheck() == BST_CHECKED;
	bool bCheckStep = m_btnCheckRedStep.GetCheck() == BST_CHECKED;
	bool bCheckPrime = m_btnCheckPrime.GetCheck() == BST_CHECKED;
	bool bCheckSmall = m_btnCheckSmall.GetCheck() == BST_CHECKED;
	bool bCheckMissing = m_btnCheckMissing.GetCheck() == BST_CHECKED;

	RedInPositionConstraint* pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_1));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed1.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR1, m_editRed1.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_1, pNumSetCt);
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_2));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed2.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR2, m_editRed2.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_2, pNumSetCt);
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_3));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed3.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR3, m_editRed3.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_3, pNumSetCt);
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_4));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed4.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR4, m_editRed4.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_4, pNumSetCt);
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_5));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed5.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR5, m_editRed5.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_5, pNumSetCt);
	}

	pNumSetCt = dynamic_cast<RedInPositionConstraint*>(condition.GetConstraint(VALID_RED_6));
	if (pNumSetCt != NULL)
	{
		pNumSetCt->SetSuppressed(!bCheckRed);
		pNumSetCt->SetNumSet(m_editRed6.GetNumSet());
	}
	else
	{
		pNumSetCt = new RedInPositionConstraint(kR6, m_editRed6.GetNumSet());
		pNumSetCt->SetSuppressed(!bCheckRed);
		condition.AddConstraint(VALID_RED_6, pNumSetCt);
	}

	RedNumbersConstraint* pRedCt = dynamic_cast<RedNumbersConstraint*>(condition.GetConstraint(VALID_RED));
	if (pRedCt != NULL)
	{
		pRedCt->SetSuppressed(!bCheckRedGeneral);
		CString strText;
		m_editRedRestricts.GetWindowText(strText);
		pRedCt->SetText(strText);
	}
	else
	{
		pRedCt = new RedNumbersConstraint();
		pRedCt->SetSuppressed(!bCheckRedGeneral);
		
		CString strText;
		m_editRedRestricts.GetWindowText(strText);
		pRedCt->SetText(strText);

		condition.AddConstraint(VALID_RED, pRedCt);
	}

	RedStepConstraint* pStepCt = NULL;
	pStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_12));
	if (pStepCt != NULL)
	{
		pStepCt->SetSuppressed(!bCheckStep);
		pStepCt->SetNumSet(m_editStep12.GetNumSet());
	}
	else
	{
		pStepCt = new RedStepConstraint(kR1, kR2, m_editStep12.GetNumSet());
		pStepCt->SetSuppressed(!bCheckStep);
		condition.AddConstraint(VALID_RED_STEP_12, pStepCt);
	}

	pStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_23));
	if (pStepCt != NULL)
	{
		pStepCt->SetSuppressed(!bCheckStep);
		pStepCt->SetNumSet(m_editStep23.GetNumSet());
	}
	else
	{
		pStepCt = new RedStepConstraint(kR2, kR3, m_editStep23.GetNumSet());
		pStepCt->SetSuppressed(!bCheckStep);
		condition.AddConstraint(VALID_RED_STEP_23, pStepCt);
	}

	pStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_34));
	if (pStepCt != NULL)
	{
		pStepCt->SetSuppressed(!bCheckStep);
		pStepCt->SetNumSet(m_editStep34.GetNumSet());
	}
	else
	{
		pStepCt = new RedStepConstraint(kR3, kR4, m_editStep34.GetNumSet());
		pStepCt->SetSuppressed(!bCheckStep);
		condition.AddConstraint(VALID_RED_STEP_34, pStepCt);
	}

	pStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_45));
	if (pStepCt != NULL)
	{
		pStepCt->SetSuppressed(!bCheckStep);
		pStepCt->SetNumSet(m_editStep45.GetNumSet());
	}
	else
	{
		pStepCt = new RedStepConstraint(kR4, kR5, m_editStep45.GetNumSet());
		pStepCt->SetSuppressed(!bCheckStep);
		condition.AddConstraint(VALID_RED_STEP_45, pStepCt);
	}

	pStepCt = dynamic_cast<RedStepConstraint*>(condition.GetConstraint(VALID_RED_STEP_56));
	if (pStepCt != NULL)
	{
		pStepCt->SetSuppressed(!bCheckStep);
		pStepCt->SetNumSet(m_editStep56.GetNumSet());
	}
	else
	{
		pStepCt = new RedStepConstraint(kR5, kR6, m_editStep56.GetNumSet());
		pStepCt->SetSuppressed(!bCheckStep);
		condition.AddConstraint(VALID_RED_STEP_56, pStepCt);
	}

	// properties...
	NumberSumConstraint* pSumCt = dynamic_cast<NumberSumConstraint*>(condition.GetConstraint(VALID_SUM));
	if (pSumCt != NULL)
	{
		pSumCt->SetSuppressed(!bCheckSum);
		pSumCt->SetNumSet(m_editSum.GetNumSet());
	}
	else
	{
		pSumCt = new NumberSumConstraint(m_editSum.GetNumSet());
		pSumCt->SetSuppressed(!bCheckSum);
		condition.AddConstraint(VALID_SUM, pSumCt);
	}

	RepeatCountConstraint* pRepeatCt = dynamic_cast<RepeatCountConstraint*>(condition.GetConstraint(VALID_REPEAT_COUNT));
	if (pRepeatCt != NULL)
	{
		pRepeatCt->SetSuppressed(!bCheckRepeat);
		pRepeatCt->SetNumSet(m_editRepeat.GetNumSet());
	}
	else
	{
		pRepeatCt = new RepeatCountConstraint(m_editRepeat.GetNumSet());
		pRepeatCt->SetSuppressed(!bCheckRepeat);
		condition.AddConstraint(VALID_REPEAT_COUNT, pRepeatCt);
	}

	ContinuityConstraint* pContCt = dynamic_cast<ContinuityConstraint*>(condition.GetConstraint(VALID_CONTINUOUS_COUNT));
	if (pContCt != NULL)
	{
		pContCt->SetSuppressed(!bCheckConti);
		pContCt->SetNumSet(m_editContinuity.GetNumSet());
	}
	else
	{
		pContCt = new ContinuityConstraint(m_editContinuity.GetNumSet());
		pContCt->SetSuppressed(!bCheckConti);
		condition.AddConstraint(VALID_CONTINUOUS_COUNT, pContCt);
	}

	EvenCountConstraint* pEvenCt = dynamic_cast<EvenCountConstraint*>(condition.GetConstraint(VALID_EVEN_COUNT));
	if (pEvenCt != NULL)
	{
		pEvenCt->SetSuppressed(!bCheckEven);
		pEvenCt->SetNumSet(m_editEven.GetNumSet());
	}
	else
	{
		pEvenCt = new EvenCountConstraint(m_editEven.GetNumSet());
		pEvenCt->SetSuppressed(!bCheckEven);
		condition.AddConstraint(VALID_EVEN_COUNT, pEvenCt);
	}

	PrimeNumConstraint* pPrimeCt = dynamic_cast<PrimeNumConstraint*>(condition.GetConstraint(VALID_PRIME_NUM_COUNT));
	if (pPrimeCt != NULL)
	{
		pPrimeCt->SetSuppressed(!bCheckPrime);
		pPrimeCt->SetNumSet(m_editPrime.GetNumSet());
	}
	else
	{
		pPrimeCt = new PrimeNumConstraint(m_editPrime.GetNumSet());
		pPrimeCt->SetSuppressed(!bCheckPrime);
		condition.AddConstraint(VALID_PRIME_NUM_COUNT, pPrimeCt);
	}

	SmallNumConstraint* pSmallCt = dynamic_cast<SmallNumConstraint*>(condition.GetConstraint(VALID_SMALL_NUM_COUNT));
	if (pSmallCt != NULL)
	{
		pSmallCt->SetSuppressed(!bCheckSmall);
		pSmallCt->SetNumSet(m_editSmall.GetNumSet());
	}
	else
	{
		pSmallCt = new SmallNumConstraint(m_editSmall.GetNumSet());
		pSmallCt->SetSuppressed(!bCheckSmall);
		condition.AddConstraint(VALID_SMALL_NUM_COUNT, pSmallCt);
	}

	TotalMissingConstraint* pMissingCt = dynamic_cast<TotalMissingConstraint*>(condition.GetConstraint(VALID_TOTAL_MISSING));
	if (pMissingCt != NULL)
	{
		pMissingCt->SetSuppressed(!bCheckMissing);
		pMissingCt->SetNumSet(m_editMissing.GetNumSet());
	}
	else
	{
		pMissingCt = new TotalMissingConstraint(m_editMissing.GetNumSet());
		pMissingCt->SetSuppressed(!bCheckMissing);
		condition.AddConstraint(VALID_TOTAL_MISSING, pMissingCt);
	}

	return true;
}

void CCalculateDlg::OnLvnGetdispinfoListResult(NMHDR *pNMHDR, LRESULT *pResult)
{
	LV_DISPINFO* pDispInfo = (LV_DISPINFO*)pNMHDR; 
	LV_ITEM* pItem= &(pDispInfo)-> item;

	int iItemIndx= pItem->iItem; 

	if (pItem->mask & LVIF_TEXT)   //valid   text   buffer? 
	{
		if (iItemIndx < (int)m_CalResult.size())
		{
			CString strTemp;
			if (pItem->iSubItem == 0)
				strTemp.Format(_T("%d"), iItemIndx + 1);
			else
				strTemp.Format(_T("%d"), m_CalResult[iItemIndx]->m_red[pItem-> iSubItem - 1]);
			lstrcpy(pItem-> pszText, strTemp);
		}
	}

	*pResult = 0;
}


void CCalculateDlg::OnBnClickedButtonAsDefault()
{
	Constraints defaultConsts;
	if (defaultConsts.AsDefault())
	{
		this->SetConditionCtrls(defaultConsts);
	}
}

void CCalculateDlg::OnBnClickedButtonImport()
{
	// "*.my" for "MyType Files" and "*.*' for "All Files."
	CString strFileType= _T("Xml Files (*.xml)|*.lns|All Files (*.*)|*.*||");

	// Create an Open dialog; the default file name extension is ".my".
	CFileDialog fileDlg(TRUE, _T("xml"), _T("*.xml"), OFN_FILEMUSTEXIST| OFN_HIDEREADONLY, strFileType, this);
	
	// Display the file dialog. When user clicks OK, fileDlg.DoModal() 
	// returns IDOK.
	if( fileDlg.DoModal()==IDOK )
	{
		CString pathName = fileDlg.GetPathName();

		Constraints constraints;
		if (constraints.ReadFrom(pathName))
		{
			this->SetConditionCtrls(constraints);
		}
	}
}

void CCalculateDlg::OnBnClickedButtonExport()
{
	// "*.my" for "MyType Files" and "*.*' for "All Files."
	CString strFileType= _T("Xml Files (*.xml)|*.xml|All Files (*.*)|*.*||");

	// Create an Open dialog; the default file name extension is ".my".
	CFileDialog fileDlg(FALSE, _T("xml"), _T("*.xml"), OFN_FILEMUSTEXIST| OFN_HIDEREADONLY, strFileType, this);
	
	// Display the file dialog. When user clicks OK, fileDlg.DoModal() 
	// returns IDOK.
	if( fileDlg.DoModal()==IDOK )
	{
		CString pathName = fileDlg.GetPathName();

		Constraints constraints;
		if (this->GetConstraints(constraints))
		{
			constraints.WriteTo(pathName);
		}
	}
}

void CCalculateDlg::OnBnClickedButtonEditRed()
{
	CString strRestricts;
	m_editRedRestricts.GetWindowText(strRestricts);

	RedNumbersConstraint con;
	con.SetText(strRestricts);

	CNumRestrictsDlg dlg(&con, this);
	dlg.DoModal();

	m_editRedRestricts.SetWindowText(con.GetText());
}


void CCalculateDlg::OnBnClickedCheckMatrix()
{
	// TODO: Add your control notification handler code here
}


void CCalculateDlg::OnBnClickedRadio645()
{
	// TODO: Add your control notification handler code here
}


void CCalculateDlg::OnBnClickedRadio545()
{
	// TODO: Add your control notification handler code here
}


void CCalculateDlg::OnBnClickedButtonMatrixFilter()
{
	int countHit = m_btnRadio644.GetCheck() ? 4 : 5;
	CString nums;
	m_editRedSelection.GetWindowText(nums);
	CNumSet set(nums);

	if (m_btnFindInResult.GetCheck() == BST_UNCHECKED)
	{
		// Clear the previous result.
		CAnalysisUtil::DeleteAll(m_CalResult);

		if (!CAnalysisUtil::Matrixing(set, countHit, m_CalResult))
			return;
	}
	else
	{
		if (!CAnalysisUtil::MatrixingInResult(countHit, m_CalResult))
			return;
	}

	m_ResultList.DeleteAllItems();

	int iSize = (int)m_CalResult.size();
	m_ResultList.SetItemCount(iSize);
}


void CCalculateDlg::OnBnClickedButtonStratgiesTest()
{
	const LuckyNums& history = m_pHistory->GetHistory();

	CStratgiesTestDlg reportDlg(this);
	TestResults& result = reportDlg.GetResult();

	int iRange = 5;
	switch (m_TestRange.GetCurSel())
	{
	case 0: iRange = 5; break;
	case 1: iRange = 10; break;
	case 2: iRange = 30; break;
	case 3: iRange = 50; break;
	case 4: iRange = 100; break;
	case 5: iRange = 200; break;
	}

	const CLuckyNum* pLast = m_pHistory->GetLastNum();

	int start = (int)history.size() - iRange;
	LuckyNums::const_iterator itPrev = history.end();
	for (LuckyNums::const_iterator it = history.begin(); it != history.end(); itPrev = it, ++ it)
	{
		if (-- start >= 0)
			continue;

		CTestResult* item = new CTestResult(it->first);
		item->m_LuckyNum.m_red[0] = it->second->m_red[0];
		item->m_LuckyNum.m_red[1] = it->second->m_red[1];
		item->m_LuckyNum.m_red[2] = it->second->m_red[2];
		item->m_LuckyNum.m_red[3] = it->second->m_red[3];
		item->m_LuckyNum.m_red[4] = it->second->m_red[4];
		item->m_LuckyNum.m_red[5] = it->second->m_red[5];

		//item->m_LuckyNum.m_red[0] = pLast->m_red[0];
		//item->m_LuckyNum.m_red[1] = pLast->m_red[1];
		//item->m_LuckyNum.m_red[2] = pLast->m_red[2];
		//item->m_LuckyNum.m_red[3] = pLast->m_red[3];
		//item->m_LuckyNum.m_red[4] = pLast->m_red[4];
		//item->m_LuckyNum.m_red[5] = pLast->m_red[5];

		Lucky::SetTestIssue(itPrev->first);
		if (m_checkEvaluateResult.GetCheck() == BST_CHECKED)
		{
			CAnalysisUtil::CopyTo(m_CalResult, item->m_CalResult, true);
		}
		else
		{
			Constraints condition;
			CConstraintUtil::SuggestConstraints(&condition);
			//GetConstraints(condition);

			// testing...
			//srand(it->first);
			//CNumSet selectedNums;
			//CAnalysisUtil::RandomRedNums(selectedNums, CNumSet(CRagion(1, 10)), 3);
			//CAnalysisUtil::RandomRedNums(selectedNums, CNumSet(CRagion(12, 10)), 3);
			//CAnalysisUtil::RandomRedNums(selectedNums, CNumSet(CRagion(23, 10)), 3);

			//RedNumbersConstraint* pConst = dynamic_cast<RedNumbersConstraint*>(CConstraintUtil::CreateConstraintInstance(VALID_RED));
			//CString strConstraint = _T("6|") + selectedNums.GetText();
			//pConst->SetText(strConstraint);
			//condition.AddConstraint(VALID_RED, pConst);

			CAnalysisUtil::Suggest(condition, item->m_CalResult);
		}
		Lucky::SetTestIssue(-1);

		item->Calculate();
		result.push_back(item);
	}

	reportDlg.DoModal();
}


void CCalculateDlg::OnCbnSelchangeComboTestIssues()
{
	// TODO: Add your control notification handler code here
}
