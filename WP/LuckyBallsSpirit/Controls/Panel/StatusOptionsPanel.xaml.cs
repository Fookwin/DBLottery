using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// 
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using LuckyBallsSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public enum OptionChangeEnum
    {
        CategoryChanged = 0,
        SubCategoryChanged = 1,
        ViewOptionChanged  = 2,
        ViewCountChanged = 3
    }

    public sealed partial class StatusOptionsPanel : UserControl
    {

        public delegate void OptionsChangeEventHandler(OptionChangeEnum arg);
        public event OptionsChangeEventHandler OnOptionsChangeEventHandler = null;

        private bool _initialized = false;

        private StatusSubCategory _subCat = StatusSubCategory.None;
        private StatusViewOption _viewOption = 0;
        private int _dispCount = 30;

        public StatusOptionsPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                _initialized = true;
            };
        }

        public StatusSubCategory SubCategory
        {
            get
            {
                return _subCat;
            }
            set
            {
                RefreshSubCategoryFromValue(value);
            }
        }

        public StatusViewOption ViewOption
        {
            get
            {
                return _viewOption;
            }
            set
            {
                RefreshViewOptionFromValue(value);
            }
        }

        public int ViewIssueCount
        {
            get
            {
                return _dispCount;
            }
            set
            {
                RefreshViewCountFromValue(value);
            }
        }

        private void RefreshSubCategoryFromValue(StatusSubCategory val)
        {
            _initialized = false;

            if (val == StatusSubCategory.None)
            {
                // Uncheck all.
                RB_Div_3.IsChecked = false;
                RB_Div_4.IsChecked = false;
                RB_Div_7.IsChecked = false;
                RB_Div_11.IsChecked = false;
                RB_RedPos_1.IsChecked = false;
                RB_RedPos_2.IsChecked = false;
                RB_RedPos_3.IsChecked = false;
                RB_RedPos_4.IsChecked = false;
                RB_RedPos_5.IsChecked = false;
                RB_RedPos_6.IsChecked = false;
            }
            else
            {
                switch (val)
                {
                    case StatusSubCategory.RedDivisionIn3: RB_Div_3.IsChecked = true; break;
                    case StatusSubCategory.RedDivisionIn4: RB_Div_4.IsChecked = true; break;
                    case StatusSubCategory.RedDivisionIn7: RB_Div_7.IsChecked = true; break;
                    case StatusSubCategory.RedDivisionIn11: RB_Div_11.IsChecked = true; break;
                    case StatusSubCategory.RedPosition1: RB_RedPos_1.IsChecked = true; break;
                    case StatusSubCategory.RedPosition2: RB_RedPos_2.IsChecked = true; break;
                    case StatusSubCategory.RedPosition3: RB_RedPos_3.IsChecked = true; break;
                    case StatusSubCategory.RedPosition4: RB_RedPos_4.IsChecked = true; break;
                    case StatusSubCategory.RedPosition5: RB_RedPos_5.IsChecked = true; break;
                    case StatusSubCategory.RedPosition6: RB_RedPos_6.IsChecked = true; break;
                }
            }

            _subCat = val;

            _initialized = true;
        }

        private void RefreshViewOptionFromValue(StatusViewOption val)
        {
            _initialized = false;

            switch (val)
            {
                case StatusViewOption.None:
                    {
                        RB_None_Con.IsChecked = true;
                        RB_None_Detail.IsChecked = true;
                        break;
                    }
                case StatusViewOption.RedHorizantalConnection: RB_HOR_Con.IsChecked = true; break;
                case StatusViewOption.RedVerticalConnection: RB_VRL_Con.IsChecked = true; break;
                case StatusViewOption.RedObliqueConnection: RB_OLQ_Con.IsChecked = true; break;
                case StatusViewOption.RedOddConnection: RB_ODD_Con.IsChecked = true; break;
                case StatusViewOption.RedEvenConnection: RB_EVN_Con.IsChecked = true; break;
                case StatusViewOption.RedSumDetail: RB_SUM_Detail.IsChecked = true; break;
                case StatusViewOption.RedContinuityDetail: RB_CON_Detail.IsChecked = true; break;
                case StatusViewOption.RedEvenOddDetail: RB_ONE_Detail.IsChecked = true; break;
                case StatusViewOption.RedPrimaryCompositeDetail: RB_PNC_Detail.IsChecked = true; break;
                case StatusViewOption.RedBigSmallDetail: RB_BNS_Detail.IsChecked = true; break;
                case StatusViewOption.RedRemain0Detail: RB_RM0_Detail.IsChecked = true; break;
                case StatusViewOption.RedRemain1Detail: RB_RM1_Detail.IsChecked = true; break;
                case StatusViewOption.RedRemain2Detail: RB_RM2_Detail.IsChecked = true; break;
                case StatusViewOption.RedDiv1Detail: RB_DV1_Detail.IsChecked = true; break;
                case StatusViewOption.RedDiv2Detail: RB_DV2_Detail.IsChecked = true; break;
                case StatusViewOption.RedDiv3Detail: RB_DV3_Detail.IsChecked = true; break;
            }                           
                                        
            _viewOption = val;

            _initialized = true;
        }

        private void RefreshViewCountFromValue(int cnt)
        {
            _initialized = false;

            if (cnt == 30)
            {
                RB_ListCnt_30.IsChecked = true;
            }
            else if (cnt == 50)
            {
                RB_ListCnt_50.IsChecked = true;
            }
            else if (cnt == 100)
            {
                RB_ListCnt_100.IsChecked = true;
            }
            else
            {
                // not supported yet.
            }

            _dispCount = cnt;

            _initialized = true;
        }

        public void SwitchPanel(StatusCategory category, StatusOptions defaultOption)
        {
            if (category == StatusCategory.RedDivision)
            {
                RedDivPanel.Visibility = Visibility.Visible;
                RedPositionPanel.Visibility = Visibility.Collapsed;
                RedDetailExpendPanel.Visibility = Visibility.Collapsed;

                SubCategory = defaultOption != null ? defaultOption.SubCategory : StatusSubCategory.RedDivisionIn3;
                ViewOption = defaultOption != null ? defaultOption.ViewOption : StatusViewOption.None;                    
            }
            else if (category == StatusCategory.RedPosition)
            {
                RedDivPanel.Visibility = Visibility.Collapsed;
                RedPositionPanel.Visibility = Visibility.Visible;
                RedDetailExpendPanel.Visibility = Visibility.Collapsed;

                SubCategory = defaultOption != null ? defaultOption.SubCategory : StatusSubCategory.RedPosition1;
            }
            else if (category == StatusCategory.RedGeneral)
            {
                RedDivPanel.Visibility = Visibility.Collapsed;
                RedPositionPanel.Visibility = Visibility.Collapsed;
                RedDetailExpendPanel.Visibility = Visibility.Visible;

                ViewOption = defaultOption != null ? defaultOption.ViewOption : StatusViewOption.None; 
            }
            else
            {
                // hide all controls.
                RedDivPanel.Visibility = Visibility.Collapsed;
                RedPositionPanel.Visibility = Visibility.Collapsed;
                RedDetailExpendPanel.Visibility = Visibility.Collapsed;
            }

            if (defaultOption != null)
            {
                ViewIssueCount = defaultOption.ViewIssueCount;
            }
        }

        private void RedDiv_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                if (RB_Div_3.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedDivisionIn3;
                }
                else if (RB_Div_4.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedDivisionIn4;
                }
                else if (RB_Div_7.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedDivisionIn7;
                }
                else if (RB_Div_11.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedDivisionIn11;
                }

                if (OnOptionsChangeEventHandler != null)
                    OnOptionsChangeEventHandler(OptionChangeEnum.SubCategoryChanged);
            }
        }

        private void RedCon_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                if (RB_EVN_Con.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedEvenConnection;
                }
                else if (RB_HOR_Con.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedHorizantalConnection;
                }
                else if (RB_VRL_Con.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedVerticalConnection;
                }
                else if (RB_OLQ_Con.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedObliqueConnection;
                }
                else if (RB_ODD_Con.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedOddConnection;
                }
                else if (RB_None_Con.IsChecked.Value)
                {
                    _viewOption = 0;
                }

                if (OnOptionsChangeEventHandler != null)
                    OnOptionsChangeEventHandler(OptionChangeEnum.ViewOptionChanged);
            }
        }

        private void RedDetail_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                if (RB_SUM_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedSumDetail;
                }
                else if (RB_CON_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedContinuityDetail;
                }
                else if (RB_ONE_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedEvenOddDetail;
                }
                else if (RB_PNC_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedPrimaryCompositeDetail;
                }
                else if (RB_BNS_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedBigSmallDetail;
                }
                else if (RB_RM0_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedRemain0Detail;
                }
                else if (RB_RM1_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedRemain1Detail;
                }
                else if (RB_RM2_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedRemain2Detail;
                }
                else if (RB_DV1_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedDiv1Detail;
                }
                else if (RB_DV2_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedDiv2Detail;
                }
                else if (RB_DV3_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.RedDiv3Detail;
                }
                else if (RB_None_Detail.IsChecked.Value)
                {
                    _viewOption = StatusViewOption.None;
                }

                if (OnOptionsChangeEventHandler != null)
                    OnOptionsChangeEventHandler(OptionChangeEnum.ViewOptionChanged);
            }
        }

        private void RedPos_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                if (RB_RedPos_1.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition1;
                }
                else if (RB_RedPos_2.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition2;
                }
                else if (RB_RedPos_3.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition3;
                }
                else if (RB_RedPos_4.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition4;
                }
                else if (RB_RedPos_5.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition5;
                }
                else if (RB_RedPos_6.IsChecked.Value)
                {
                    _subCat = StatusSubCategory.RedPosition6;
                }

                if (OnOptionsChangeEventHandler != null)
                    OnOptionsChangeEventHandler(OptionChangeEnum.SubCategoryChanged);
            }
        }

        private void RB_ListCnt_30_Checked(object sender, RoutedEventArgs e)
        {
            _dispCount = 30;

            if (OnOptionsChangeEventHandler != null)
                OnOptionsChangeEventHandler(OptionChangeEnum.ViewCountChanged);
        }

        private void RB_ListCnt_50_Checked(object sender, RoutedEventArgs e)
        {
            _dispCount = 50;

            if (OnOptionsChangeEventHandler != null)
                OnOptionsChangeEventHandler(OptionChangeEnum.ViewCountChanged);
        }

        private void RB_ListCnt_100_Checked(object sender, RoutedEventArgs e)
        {
            _dispCount = 100;

            if (OnOptionsChangeEventHandler != null)
                OnOptionsChangeEventHandler(OptionChangeEnum.ViewCountChanged);
        }

        private void RB_Mis_Brk_Checked(object sender, RoutedEventArgs e)
        {
            if (_initialized)
            {
                _viewOption = StatusViewOption.RedObmissionBreak;

                if (OnOptionsChangeEventHandler != null)
                    OnOptionsChangeEventHandler(OptionChangeEnum.ViewOptionChanged);
            }
        }
    }
}
