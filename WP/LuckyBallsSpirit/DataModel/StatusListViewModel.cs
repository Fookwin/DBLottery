using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using LuckyBallsSpirit.DataModel;

namespace LuckyBallsSpirit.DataModel
{
    public enum StatusCategory
    {
        RedGeneral = 0,
        RedDivision = 1, 
        RedPosition = 2,
        BlueGeneral = 3,
        BlueSpan = 4
    }

    public enum StatusSubCategory
    {
        None = 0,
        RedDivisionIn3 = 1,
        RedDivisionIn4 = 2,
        RedDivisionIn7 = 3,
        RedDivisionIn11 = 4,
        RedPosition1 = 5,
        RedPosition2 = 6,
        RedPosition3 = 7,
        RedPosition4 = 8,
        RedPosition5 = 9,
        RedPosition6 = 10
    }

    public enum StatusViewOption
    {
        None = 0,
        RedHorizantalConnection,
        RedVerticalConnection,
        RedObliqueConnection,
        RedOddConnection,
        RedEvenConnection,
        RedObmissionBreak,
        RedSumDetail,
        RedContinuityDetail,
        RedEvenOddDetail,
        RedBigSmallDetail,
        RedPrimaryCompositeDetail,
        RedRemain0Detail,
        RedRemain1Detail,
        RedRemain2Detail,
        RedDiv1Detail,
        RedDiv2Detail,
        RedDiv3Detail
    }

    public class StatusOptions
    {
        public StatusCategory Category = StatusCategory.RedGeneral;
        public StatusSubCategory SubCategory = StatusSubCategory.None;
        public StatusViewOption ViewOption = StatusViewOption.None;
        public int ViewIssueCount = 30;
    }

    class DGCellModel : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool _showBall = false;
        private bool _isVisible = true;
        private Color _foreColor = ColorPicker.DarkGray;
        private Color _backColor = ColorPicker.White;
        private Color _ballColor = ColorPicker.White;
        private string _text = "";

        public int Traits
        {
            get;
            set;
        }

        public Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                if (_backColor != value)
                {
                    _backColor = value;
                    OnPropertyChanged("BackColor");
                }
            }
        }

        public Color ForeColor
        {
            get
            {
                return _foreColor;
            }
            set
            {
                if (_foreColor != value)
                {
                    _foreColor = value;
                    OnPropertyChanged("ForeColor");
                }
            }
        }

        public bool ShowAsBall
        {
            get
            {
                return _showBall;
            }

            set
            {
                if (_showBall != value)
                {
                    _showBall = value;
                    OnPropertyChanged("ShowAsBall");
                }
            }
        }

        public Color BallColor
        {
            get
            {
                return _ballColor;
            }

            set
            {
                if (_ballColor != value)
                {
                    _ballColor = value;
                    OnPropertyChanged("BallColor");
                }
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }

            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged("Text");
                }
            }
        }
    }

    class DGRedDivisionRowModel
    {
        public ObservableCollection<DGCellModel> Cells = new ObservableCollection<DGCellModel>(new DGCellModel[33]);

        public Visibility HasLine1
        {
            get;
            set;
        }

        public Visibility HasLine2
        {
            get;
            set;
        }

        public double Line1StartX
        {
            get;
            set;
        }

        public double Line1StartY
        {
            get;
            set;
        }

        public double Line1EndX
        {
            get;
            set;
        }

        public double Line1EndY
        {
            get;
            set;
        }

        public double Line2StartX
        {
            get;
            set;
        }

        public double Line2StartY
        {
            get;
            set;
        }

        public double Line2EndX
        {
            get;
            set;
        }

        public double Line2EndY
        {
            get;
            set;
        }

        public DGCellModel Cell_01
        {
            get { return Cells[0]; }
        }

        public DGCellModel Cell_02
        {
            get { return Cells[1]; }
        }

        public DGCellModel Cell_03
        {
            get { return Cells[2]; }
        }

        public DGCellModel Cell_04
        {
            get { return Cells[3]; }
        }

        public DGCellModel Cell_05
        {
            get { return Cells[4]; }
        }

        public DGCellModel Cell_06
        {
            get { return Cells[5]; }
        }

        public DGCellModel Cell_07
        {
            get { return Cells[6]; }
        }

        public DGCellModel Cell_08
        {
            get { return Cells[7]; }
        }

        public DGCellModel Cell_09
        {
            get { return Cells[8]; }
        }

        public DGCellModel Cell_10
        {
            get { return Cells[9]; }
        }

        public DGCellModel Cell_11
        {
            get { return Cells[10]; }
        }

        public DGCellModel Cell_12
        {
            get { return Cells[11]; }
        }

        public DGCellModel Cell_13
        {
            get { return Cells[12]; }
        }

        public DGCellModel Cell_14
        {
            get { return Cells[13]; }
        }

        public DGCellModel Cell_15
        {
            get { return Cells[14]; }
        }

        public DGCellModel Cell_16
        {
            get { return Cells[15]; }
        }

        public DGCellModel Cell_17
        {
            get { return Cells[16]; }
        }

        public DGCellModel Cell_18
        {
            get { return Cells[17]; }
        }

        public DGCellModel Cell_19
        {
            get { return Cells[18]; }
        }

        public DGCellModel Cell_20
        {
            get { return Cells[19]; }
        }

        public DGCellModel Cell_21
        {
            get { return Cells[20]; }
        }

        public DGCellModel Cell_22
        {
            get { return Cells[21]; }
        }

        public DGCellModel Cell_23
        {
            get { return Cells[22]; }
        }

        public DGCellModel Cell_24
        {
            get { return Cells[23]; }
        }

        public DGCellModel Cell_25
        {
            get { return Cells[24]; }
        }

        public DGCellModel Cell_26
        {
            get { return Cells[25]; }
        }

        public DGCellModel Cell_27
        {
            get { return Cells[26]; }
        }

        public DGCellModel Cell_28
        {
            get { return Cells[27]; }
        }

        public DGCellModel Cell_29
        {
            get { return Cells[28]; }
        }

        public DGCellModel Cell_30
        {
            get { return Cells[29]; }
        }

        public DGCellModel Cell_31
        {
            get { return Cells[30]; }
        }

        public DGCellModel Cell_32
        {
            get { return Cells[31]; }
        }

        public DGCellModel Cell_33
        {
            get { return Cells[32]; }
        }
    }

    class DGRedPositionRowModel : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public Scheme Data = null;
        public ObservableCollection<DGCellModel> Cells = null;

        private Visibility _hasLine1 = Visibility.Collapsed;
        private Visibility _hasLine2 = Visibility.Collapsed;
        private double _line1StartX = 0.0;
        private double _line1StartY = 0.0;
        private double _line1EndX = 0.0;
        private double _line1EndY = 0.0;
        private double _line2StartX = 0.0;
        private double _line2StartY = 0.0;
        private double _line2EndX = 0.0;
        private double _line2EndY = 0.0;

        public Visibility HasLine1
        {
            get
            {
                return _hasLine1;
            }
            set
            {
                if (_hasLine1 != value)
                {
                    _hasLine1 = value;
                    OnPropertyChanged("HasLine1");
                }
            }
        }

        public Visibility HasLine2
        {
            get
            {
                return _hasLine2;
            }
            set
            {
                if (_hasLine2 != value)
                {
                    _hasLine2 = value;
                    OnPropertyChanged("HasLine2");
                }
            }
        }

        public double Line1StartX
        {
            get
            {
                return _line1StartX;
            }
            set
            {
                if (_line1StartX != value)
                {
                    _line1StartX = value;
                    OnPropertyChanged("Line1StartX");
                }
            }
        }

        public double Line1StartY
        {
            get
            {
                return _line1StartY;
            }
            set
            {
                if (_line1StartY != value)
                {
                    _line1StartY = value;
                    OnPropertyChanged("Line1StartY");
                }
            }
        }

        public double Line1EndX
        {
            get
            {
                return _line1EndX;
            }
            set
            {
                if (_line1EndX != value)
                {
                    _line1EndX = value;
                    OnPropertyChanged("Line1EndX");
                }
            }
        }

        public double Line1EndY
        {
            get
            {
                return _line1EndY;
            }
            set
            {
                if (_line1EndY != value)
                {
                    _line1EndY = value;
                    OnPropertyChanged("Line1EndY");
                }
            }
        }

        public double Line2StartX
        {
            get
            {
                return _line2StartX;
            }
            set
            {
                if (_line2StartX != value)
                {
                    _line2StartX = value;
                    OnPropertyChanged("Line2StartX");
                }
            }
        }

        public double Line2StartY
        {
            get
            {
                return _line2StartY;
            }
            set
            {
                if (_line2StartY != value)
                {
                    _line2StartY = value;
                    OnPropertyChanged("Line2StartY");
                }
            }
        }

        public double Line2EndX
        {
            get
            {
                return _line2EndX;
            }
            set
            {
                if (_line2EndX != value)
                {
                    _line2EndX = value;
                    OnPropertyChanged("Line2EndX");
                }
            }
        }

        public double Line2EndY
        {
            get
            {
                return _line2EndY;
            }
            set
            {
                if (_line2EndY != value)
                {
                    _line2EndY = value;
                    OnPropertyChanged("Line2EndY");
                }
            }
        }

        public DGCellModel Cell_01
        {
            get { return Cells[0]; }
        }

        public DGCellModel Cell_02
        {
            get { return Cells[1]; }
        }

        public DGCellModel Cell_03
        {
            get { return Cells[2]; }
        }

        public DGCellModel Cell_04
        {
            get { return Cells[3]; }
        }

        public DGCellModel Cell_05
        {
            get { return Cells[4]; }
        }

        public DGCellModel Cell_06
        {
            get { return Cells[5]; }
        }

        public DGCellModel Cell_07
        {
            get { return Cells[6]; }
        }

        public DGCellModel Cell_08
        {
            get { return Cells[7]; }
        }

        public DGCellModel Cell_09
        {
            get { return Cells[8]; }
        }

        public DGCellModel Cell_10
        {
            get { return Cells[9]; }
        }

        public DGCellModel Cell_11
        {
            get { return Cells[10]; }
        }

        public DGCellModel Cell_12
        {
            get { return Cells[11]; }
        }

        public DGCellModel Cell_13
        {
            get { return Cells[12]; }
        }

        public DGCellModel Cell_14
        {
            get { return Cells[13]; }
        }

        public DGCellModel Cell_15
        {
            get { return Cells[14]; }
        }

        public DGCellModel Cell_16
        {
            get { return Cells[15]; }
        }

        public DGCellModel Cell_17
        {
            get { return Cells[16]; }
        }

        public DGCellModel Cell_18
        {
            get { return Cells[17]; }
        }

        public DGCellModel Even
        {
            get;
            set;
        }

        public DGCellModel Odd
        {
            get;
            set;
        }

        public DGCellModel Primary
        {
            get;
            set;
        }

        public DGCellModel Composite
        {
            get;
            set;
        }

        public DGCellModel Remain0
        {
            get;
            set;
        }

        public DGCellModel Remain1
        {
            get;
            set;
        }

        public DGCellModel Remain2
        {
            get;
            set;
        }

        public DGCellModel WXJin
        {
            get;
            set;
        }

        public DGCellModel WXMu
        {
            get;
            set;
        }

        public DGCellModel WXShui
        {
            get;
            set;
        }

        public DGCellModel WXHuo
        {
            get;
            set;
        }

        public DGCellModel WXTu
        {
            get;
            set;
        }
    }

    class DGBlueNumberRowModel
    {
        public ObservableCollection<DGCellModel> Cells = new ObservableCollection<DGCellModel>(new DGCellModel[16]);

        public Visibility HasLine1
        {
            get;
            set;
        }

        public Visibility HasLine2
        {
            get;
            set;
        }

        public double Line1StartX
        {
            get;
            set;
        }

        public double Line1StartY
        {
            get;
            set;
        }

        public double Line1EndX
        {
            get;
            set;
        }

        public double Line1EndY
        {
            get;
            set;
        }

        public double Line2StartX
        {
            get;
            set;
        }

        public double Line2StartY
        {
            get;
            set;
        }

        public double Line2EndX
        {
            get;
            set;
        }

        public double Line2EndY
        {
            get;
            set;
        }

        public DGCellModel Cell_01
        {
            get { return Cells[0]; }
        }

        public DGCellModel Cell_02
        {
            get { return Cells[1]; }
        }

        public DGCellModel Cell_03
        {
            get { return Cells[2]; }
        }

        public DGCellModel Cell_04
        {
            get { return Cells[3]; }
        }

        public DGCellModel Cell_05
        {
            get { return Cells[4]; }
        }

        public DGCellModel Cell_06
        {
            get { return Cells[5]; }
        }

        public DGCellModel Cell_07
        {
            get { return Cells[6]; }
        }

        public DGCellModel Cell_08
        {
            get { return Cells[7]; }
        }

        public DGCellModel Cell_09
        {
            get { return Cells[8]; }
        }

        public DGCellModel Cell_10
        {
            get { return Cells[9]; }
        }

        public DGCellModel Cell_11
        {
            get { return Cells[10]; }
        }

        public DGCellModel Cell_12
        {
            get { return Cells[11]; }
        }

        public DGCellModel Cell_13
        {
            get { return Cells[12]; }
        }

        public DGCellModel Cell_14
        {
            get { return Cells[13]; }
        }

        public DGCellModel Cell_15
        {
            get { return Cells[14]; }
        }

        public DGCellModel Cell_16
        {
            get { return Cells[15]; }
        }

        public DGCellModel Even
        {
            get;
            set;
        }

        public DGCellModel Odd
        {
            get;
            set;
        }

        public DGCellModel Big
        {
            get;
            set;
        }

        public DGCellModel Small
        {
            get;
            set;
        }

        public DGCellModel Primary
        {
            get;
            set;
        }

        public DGCellModel Composite
        {
            get;
            set;
        }

        public DGCellModel Remain0
        {
            get;
            set;
        }

        public DGCellModel Remain1
        {
            get;
            set;
        }

        public DGCellModel Remain2
        {
            get;
            set;
        }        

        public DGCellModel WXJin
        {
            get;
            set;
        }

        public DGCellModel WXMu
        {
            get;
            set;
        }
  
        public DGCellModel WXShui
        {
            get;
            set;
        }

        public DGCellModel WXHuo
        {
            get;
            set;
        }

        public DGCellModel WXTu
        {
            get;
            set;
        }
    }

    class DGRedGeneralRowModel : INotifyPropertyChanged
    {
        // Declare the event 
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event 
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ObservableCollection<DGCellModel> Extends = new ObservableCollection<DGCellModel>(new DGCellModel[9]);


        private Visibility _hasLine1 = Visibility.Collapsed;
        private Visibility _hasLine2 = Visibility.Collapsed;
        private double _line1StartX = 0.0;
        private double _line1StartY = 0.0;
        private double _line1EndX = 0.0;
        private double _line1EndY = 0.0;
        private double _line2StartX = 0.0;
        private double _line2StartY = 0.0;
        private double _line2EndX = 0.0;
        private double _line2EndY = 0.0;

        public Visibility HasLine1
        {
            get
            {
                return _hasLine1;
            }
            set
            {
                if (_hasLine1 != value)
                {
                    _hasLine1 = value;
                    OnPropertyChanged("HasLine1");
                }
            }
        }

        public Visibility HasLine2
        {
            get
            {
                return _hasLine2;
            }
            set
            {
                if (_hasLine2 != value)
                {
                    _hasLine2 = value;
                    OnPropertyChanged("HasLine2");
                }
            }
        }

        public double Line1StartX
        {
            get
            {
                return _line1StartX;
            }
            set
            {
                if (_line1StartX != value)
                {
                    _line1StartX = value;
                    OnPropertyChanged("Line1StartX");
                }
            }
        }

        public double Line1StartY
        {
            get
            {
                return _line1StartY;
            }
            set
            {
                if (_line1StartY != value)
                {
                    _line1StartY = value;
                    OnPropertyChanged("Line1StartY");
                }
            }
        }

        public double Line1EndX
        {
            get
            {
                return _line1EndX;
            }
            set
            {
                if (_line1EndX != value)
                {
                    _line1EndX = value;
                    OnPropertyChanged("Line1EndX");
                }
            }
        }

        public double Line1EndY
        {
            get
            {
                return _line1EndY;
            }
            set
            {
                if (_line1EndY != value)
                {
                    _line1EndY = value;
                    OnPropertyChanged("Line1EndY");
                }
            }
        }

        public double Line2StartX
        {
            get
            {
                return _line2StartX;
            }
            set
            {
                if (_line2StartX != value)
                {
                    _line2StartX = value;
                    OnPropertyChanged("Line2StartX");
                }
            }
        }

        public double Line2StartY
        {
            get
            {
                return _line2StartY;
            }
            set
            {
                if (_line2StartY != value)
                {
                    _line2StartY = value;
                    OnPropertyChanged("Line2StartY");
                }
            }
        }

        public double Line2EndX
        {
            get
            {
                return _line2EndX;
            }
            set
            {
                if (_line2EndX != value)
                {
                    _line2EndX = value;
                    OnPropertyChanged("Line2EndX");
                }
            }
        }

        public double Line2EndY
        {
            get
            {
                return _line2EndY;
            }
            set
            {
                if (_line2EndY != value)
                {
                    _line2EndY = value;
                    OnPropertyChanged("Line2EndY");
                }
            }
        }

        public DGCellModel Extend1
        {
            get
            {
                return Extends[0];
            }
        }

        public DGCellModel Extend2
        {
            get
            {
                return Extends[1];
            }
        }

        public DGCellModel Extend3
        {
            get
            {
                return Extends[2];
            }
        }

        public DGCellModel Extend4
        {
            get
            {
                return Extends[3];
            }
        }

        public DGCellModel Extend5
        {
            get
            {
                return Extends[4];
            }
        }
        public DGCellModel Extend6
        {
            get
            {
                return Extends[5];
            }
        }

        public DGCellModel Extend7
        {
            get
            {
                return Extends[6];
            }
        }

        public DGCellModel Extend8
        {
            get
            {
                return Extends[7];
            }
        }

        public DGCellModel Extend9
        {
            get
            {
                return Extends[8];
            }
        }

        public DGCellModel Even
        {
            get;
            set;
        }

        public DGCellModel Odd
        {
            get;
            set;
        }

        public DGCellModel Big
        {
            get;
            set;
        }

        public DGCellModel Small
        {
            get;
            set;
        }

        public DGCellModel Primary
        {
            get;
            set;
        }

        public DGCellModel Composite
        {
            get;
            set;
        }

        public DGCellModel Remain0
        {
            get;
            set;
        }

        public DGCellModel Remain1
        {
            get;
            set;
        }

        public DGCellModel Remain2
        {
            get;
            set;
        }

        public DGCellModel Sum
        {
            get;
            set;
        }

        public DGCellModel Continuity
        {
            get;
            set;
        }

        public DGCellModel Div1
        {
            get;
            set;
        }

        public DGCellModel Div2
        {
            get;
            set;
        }

        public DGCellModel Div3
        {
            get;
            set;
        }
    }

    abstract class DGViewModelBase
    {
        public abstract void Init(List<Lottery> lotteries);
        public abstract object GetViewData();
        public abstract void SetOptions(StatusOptions op);
    }

    class DGRedDivisionViewModel : DGViewModelBase
    {
        private ObservableCollection<DGRedDivisionRowModel> _viewData = null;

        private static Color c_NormalBallColor = ColorPicker.Red;
        private static Color c_HighlightBallColor = ColorPicker.DarkGoldenrod;
        private static Color c_LightForeColor = ColorPicker.White;
        private static Color c_DarkForeColor = ColorPicker.LightGray;
        private static Color c_TransparentColor = ColorPicker.Transparent;
        private static Color c_EvenColBackColor = ColorPicker.LightGoldenrodYellow;
        private static Color c_OddColBackColor = ColorPicker.White;
        private static Color c_DimBackColor = ColorPicker.Gray;

        public override void SetOptions(StatusOptions op)
        {
            if (_viewData == null)
                return;

            int _divGroupCount = DivisionCountFromType(op.SubCategory);

            UInt64 divMash = 0;
            if (op.ViewOption == StatusViewOption.RedObmissionBreak)
            {
                if (_divGroupCount == 4)
                {
                    divMash = ((UInt64)1 << 5) - 1;
                    if (op.SubCategory == StatusSubCategory.RedDivisionIn4)
                        divMash &= ~((UInt64)4);
                }
                else
                {
                    divMash = ((UInt64)1 << _divGroupCount) - 1;
                }
            }

            // Update data.
            foreach (DGRedDivisionRowModel row in _viewData)
            {
                UInt64 misBreaks = divMash;

                for (int i = 1; i <= 33; ++i)
                {
                    DGCellModel cell = row.Cells[i - 1];

                    int divIndex = GetDivisionIndex(i, _divGroupCount);
                    cell.BackColor = (divIndex % 2 == 1) ? c_EvenColBackColor : c_OddColBackColor;

                    if ((cell.Traits & (int)RedDivsionTrait.Hitted) != 0)
                    {
                        if ((op.ViewOption == StatusViewOption.RedHorizantalConnection && (cell.Traits & (int)RedDivsionTrait.HConnected) != 0) ||
                            (op.ViewOption == StatusViewOption.RedVerticalConnection && (cell.Traits & (int)RedDivsionTrait.VConnected) != 0) ||
                            (op.ViewOption == StatusViewOption.RedObliqueConnection && (cell.Traits & (int)RedDivsionTrait.OConnected) != 0) ||
                            (op.ViewOption == StatusViewOption.RedOddConnection && (cell.Traits & (int)RedDivsionTrait.OddConnected) != 0) ||
                            (op.ViewOption == StatusViewOption.RedEvenConnection && (cell.Traits & (int)RedDivsionTrait.EvenConnected) != 0)
                            )
                        {                            
                            cell.BallColor = c_HighlightBallColor;
                        }
                        else 
                        {
                            cell.BallColor = c_NormalBallColor;

                            if (op.ViewOption == StatusViewOption.RedObmissionBreak)
                            {
                                misBreaks &= ~((UInt64)1 << (divIndex - 1));
                            }
                        }

                        cell.ForeColor = c_LightForeColor;
                        cell.ShowAsBall = true;
                    }
                    else
                    {
                        cell.ForeColor = c_DarkForeColor;
                        cell.BallColor = c_TransparentColor;
                        cell.ShowAsBall = false;                        
                    } 
                }

                if (op.ViewOption == StatusViewOption.RedObmissionBreak && misBreaks != 0)
                {
                    for (int i = 1; i <= 33; ++i)
                    {
                        int divIndex = GetDivisionIndex(i, _divGroupCount);

                        if ((misBreaks & ((UInt64)1 << (divIndex - 1))) != 0)
                        {
                            DGCellModel cell = row.Cells[i - 1];
                            cell.BackColor = c_DimBackColor;
                        }
                    }
                }
            }
        }

        public override object GetViewData()
        {
            return _viewData;
        }

        public override void Init(List<Lottery> lotteries)
        {
            if (_viewData == null)
                _viewData = new ObservableCollection<DGRedDivisionRowModel>();

            int count = lotteries.Count;

            // Initialize the connection detectors.
            ConnectionDetector detector = new ConnectionDetector(lotteries.Count);            
            
            for (int index = 0; index < count; ++ index)
            {
                Lottery lot = lotteries[index];
 
                DGRedDivisionRowModel rowLine = new DGRedDivisionRowModel();

                for (int i = 0; i < 33; ++i)
                {
                    rowLine.Cells[i] = new DGCellModel();

                    if (lot.Scheme.Contains(i + 1))
                    {
                        rowLine.Cells[i].Text = (i + 1).ToString().PadLeft(2, '0');
                        rowLine.Cells[i].Traits |= (int)RedDivsionTrait.Hitted;

                        // Do connection detection
                        detector.SetTest(i, index, rowLine.Cells[i]);
                    }
                    else
                    {
                        rowLine.Cells[i].Text = lot.Status.RedNumStates[i].Omission.ToString();
                    }
                }

                _viewData.Add(rowLine);
            }

            detector.Detect();
        }

        public static int DivisionCountFromType(StatusSubCategory cat)
        {
            int divCount = -1;
            switch (cat)
            {
                case StatusSubCategory.RedDivisionIn3:
                    divCount = 3;
                    break;
                case StatusSubCategory.RedDivisionIn4:
                    divCount = 4;
                    break;
                case StatusSubCategory.RedDivisionIn7:
                    divCount = 7;
                    break;
                case StatusSubCategory.RedDivisionIn11:
                    divCount = 11;
                    break;
            }

            return divCount;
        }        

        private static int GetDivisionIndex(int num, int groupCount)
        {
            int divAt = 0;
            if (groupCount == 3)
                divAt = (num - 1) / 11 + 1;
            else if (groupCount == 4)
                divAt = num < 17 ? (num - 1) / 8 + 1 : (num == 17 ? 3 : (num - 18) / 8 + 4);
            else if (groupCount == 7)
                divAt = (num - 1) / 5 + 1;
            else if (groupCount == 11)
                divAt = (num - 1) / 3 + 1;

            return divAt; 
        }  
    }

    class DGBlueGeneralViewModel : DGViewModelBase
    {
        enum BlueTrait
        {
            Hitted = 1,
        }

        private ObservableCollection<DGBlueNumberRowModel> _viewData = null;

        private static Color c_BallColor = ColorPicker.Blue;
        private static Color c_LightForeColor = ColorPicker.White;
        private static Color c_DarkForeColor = ColorPicker.LightGray;
        private static Color c_DimForeColor = ColorPicker.DarkGray;
        private static Color c_PrimaryBackColor = ColorPicker.White;
        private static Color c_SecondaryBackColor = ColorPicker.LightGoldenrodYellow;
        private static Color c_PropertyAltBackColor = ColorPicker.Azure;

        public override object GetViewData()
        {
            return _viewData;
        }

        public override void SetOptions(StatusOptions op)
        {
            if (_viewData == null)
                return;

            foreach (DGBlueNumberRowModel row in _viewData)
            {
                for (int i = 1; i <= 16; ++i)
                {
                    DGCellModel cell = row.Cells[i - 1];

                    // "ForeColor"
                    if ((cell.Traits & (int)BlueTrait.Hitted) != 0)
                    {
                        cell.ForeColor = c_LightForeColor;
                    }
                    else
                    {
                        cell.ForeColor = c_DarkForeColor;
                    }

                    // "ShowAsBall"
                    cell.ShowAsBall = (cell.Traits & (int)BlueTrait.Hitted) != 0;

                    // "BallColor"
                    cell.BallColor = c_BallColor;
                }
            }
        }

        public override void Init(List<Lottery> lotteries)
        {
            _viewData = new ObservableCollection<DGBlueNumberRowModel>();

            DGBlueNumberRowModel preRow = null;

            foreach (Lottery lot in lotteries)
            {
                DGBlueNumberRowModel rowLine = new DGBlueNumberRowModel();

                for (int i = 0; i < 16; ++i)
                {
                    int traits = 0;
                    string text = "";

                    if (lot.Scheme.Blue == i + 1)
                    {
                        text = (i + 1).ToString().PadLeft(2, '0');
                        traits |= (int)BlueTrait.Hitted;

                        rowLine.Line1StartY = 0;
                        rowLine.Line2EndY = 25;
                        rowLine.Line1EndX = 25 * i + 12.5;
                        rowLine.Line1EndY = 12.5;
                        rowLine.Line2StartX = 25 * i + 12.5;
                        rowLine.Line2StartY = 12.5;

                        rowLine.HasLine1 = Visibility.Collapsed;
                        rowLine.HasLine2 = Visibility.Collapsed;

                        if (preRow != null)
                        {
                            // line 2 for previous row.
                            preRow.HasLine2 = Visibility.Visible;
                            preRow.Line2EndX = (preRow.Line2StartX + rowLine.Line1EndX) / 2;

                            // Line1 for this row.
                            rowLine.HasLine1 = Visibility.Visible;
                            rowLine.Line1StartX = preRow.Line2EndX;

                            // Adjust line
                            double startX = preRow.Line2StartX;
                            double startY = preRow.Line2StartY;
                            double endX = rowLine.Line1EndX;
                            double endY = rowLine.Line1EndY;

                            AdjustLine(ref startX, ref startY, ref endX, ref endY);

                            preRow.Line2StartX = startX;
                            preRow.Line2StartY = startY;
                            rowLine.Line1EndX = endX;
                            rowLine.Line1EndY = endY;
                        }
                    }
                    else
                    {
                        text = lot.Status.BlueNumStates[i].Omission.ToString();
                    }

                    rowLine.Cells[i] = new DGCellModel();
                    rowLine.Cells[i].Traits = traits;
                    rowLine.Cells[i].Text = text;
                    rowLine.Cells[i].BackColor = (((i / 4) % 2) == 1) ? c_PrimaryBackColor : c_SecondaryBackColor;
                }

                // Add properties.
                bool IsEven = (lot.Scheme.Blue % 2 == 0);
                rowLine.Odd = new DGCellModel() { Text = IsEven ? "" : "奇", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Even = new DGCellModel() { Text = IsEven ? "偶" : "", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                bool IsBig = lot.Scheme.Blue >= 8;
                rowLine.Small = new DGCellModel() { Text = IsBig ? "" : "小", ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Big = new DGCellModel() { Text = IsBig ? "大" : "", ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                bool IsPrimary = AttributeUtil.IsPrime(lot.Scheme.Blue);
                rowLine.Composite = new DGCellModel() { Text = IsPrimary ? "" : "合", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Primary = new DGCellModel() { Text = IsPrimary ? "质" : "", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                int remain = (lot.Scheme.Blue % 3);
                rowLine.Remain0 = new DGCellModel() { Text = (remain == 0 ? "0" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Remain1 = new DGCellModel() { Text = (remain == 1 ? "1" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Remain2 = new DGCellModel() { Text = (remain == 2 ? "2" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                int wxIndex = AttributeUtil.IndexOf5Xing(lot.Scheme.Blue);
                rowLine.WXJin = new DGCellModel() { Text = (wxIndex == 1 ? "金" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXMu = new DGCellModel() { Text = (wxIndex == 2 ? "木" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXShui = new DGCellModel() { Text = (wxIndex == 3 ? "水" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXHuo = new DGCellModel() { Text = (wxIndex == 4 ? "火" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXTu = new DGCellModel() { Text = (wxIndex == 5 ? "土" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                _viewData.Add(rowLine);

                preRow = rowLine;
            }
        }

        private static void AdjustLine(ref double startX, ref double startY, ref double endX, ref double endY)
        {
            double length = Math.Sqrt(Math.Pow(endX - startX, 2) + 625);
            double offsetX = 10 * (endX - startX) / length;
            double offsetY = 250 / length;

            startX += offsetX;
            startY += offsetY;
            endX -= offsetX;
            endY -= offsetY;
        }
    }

    class DGBlueSpanViewModel : DGViewModelBase
    {
        enum BlueTrait
        {
            Hitted = 1,
        }

        private ObservableCollection<DGBlueNumberRowModel> _viewData = null;

        private static Color c_BallColor = ColorPicker.Blue;
        private static Color c_LightForeColor = ColorPicker.White;
        private static Color c_DarkForeColor = ColorPicker.LightGray;
        private static Color c_DimForeColor = ColorPicker.DarkGray;
        private static Color c_PrimaryBackColor = ColorPicker.White;
        private static Color c_SecondaryBackColor = ColorPicker.LightGoldenrodYellow;
        private static Color c_PropertyAltBackColor = ColorPicker.Azure;

        public override object GetViewData()
        {
            return _viewData;
        }

        public override void SetOptions(StatusOptions op)
        {
            if (_viewData == null)
                return;

            foreach (DGBlueNumberRowModel row in _viewData)
            {
                for (int i = 0; i < 16; ++i)
                {
                    DGCellModel cell = row.Cells[i];

                    // "ForeColor"
                    if ((cell.Traits & (int)BlueTrait.Hitted) != 0)
                    {
                        cell.ForeColor = c_LightForeColor;
                    }
                    else
                    {
                        cell.ForeColor = c_DarkForeColor;
                    }

                    // "ShowAsBall"
                    cell.ShowAsBall = (cell.Traits & (int)BlueTrait.Hitted) != 0;

                    // "BallColor"
                    cell.BallColor = c_BallColor;
                }
            }
        }

        public override void Init(List<Lottery> lotteries)
        {
            _viewData = new ObservableCollection<DGBlueNumberRowModel>();

            DGBlueNumberRowModel preRow = null;

            // get previous lottery from manager.
            Lottery first = lotteries.First();
            var allLots = LBDataManager.GetInstance().History.Lotteries;
            int indexOfFirst = allLots.IndexOf(first);
            Lottery previous = indexOfFirst > 0 ? allLots[indexOfFirst - 1] : null;

            foreach (Lottery lot in lotteries)
            {
                DGBlueNumberRowModel rowLine = new DGBlueNumberRowModel();

                // Get the span.
                int span = 0;
                if (previous != null)
                {
                    span = Math.Abs(lot.Scheme.Blue - previous.Scheme.Blue);
                }

                for (int i = 0; i < 16; ++i)
                {
                    int traits = 0;
                    string text = "";

                    if (span == i)
                    {
                        text = span.ToString().PadLeft(2, '0');
                        traits |= (int)BlueTrait.Hitted;

                        rowLine.Line1StartY = 0;
                        rowLine.Line2EndY = 25;
                        rowLine.Line1EndX = 25 * i + 12.5;
                        rowLine.Line1EndY = 12.5;
                        rowLine.Line2StartX = 25 * i + 12.5;
                        rowLine.Line2StartY = 12.5;

                        rowLine.HasLine1 = Visibility.Collapsed;
                        rowLine.HasLine2 = Visibility.Collapsed;

                        if (preRow != null)
                        {
                            // line 2 for previous row.
                            preRow.HasLine2 = Visibility.Visible;
                            preRow.Line2EndX = (preRow.Line2StartX + rowLine.Line1EndX) / 2;

                            // Line1 for this row.
                            rowLine.HasLine1 = Visibility.Visible;
                            rowLine.Line1StartX = preRow.Line2EndX;

                            // Adjust line
                            double startX = preRow.Line2StartX;
                            double startY = preRow.Line2StartY;
                            double endX = rowLine.Line1EndX;
                            double endY = rowLine.Line1EndY;

                            AdjustLine(ref startX, ref startY, ref endX, ref endY);

                            preRow.Line2StartX = startX;
                            preRow.Line2StartY = startY;
                            rowLine.Line1EndX = endX;
                            rowLine.Line1EndY = endY;
                        }
                    }

                    rowLine.Cells[i] = new DGCellModel();
                    rowLine.Cells[i].Traits = traits;
                    rowLine.Cells[i].Text = text;
                    rowLine.Cells[i].BackColor = c_PrimaryBackColor;

                    previous = lot;
                }

                // Add properties.
                bool IsEven = (span % 2 == 0);
                rowLine.Odd = new DGCellModel() { Text = IsEven ? "" : "奇", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Even = new DGCellModel() { Text = IsEven ? "偶" : "", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                bool IsBig = span >= 8;
                rowLine.Small = new DGCellModel() { Text = IsBig ? "" : "小", ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Big = new DGCellModel() { Text = IsBig ? "大" : "", ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                bool IsPrimary = AttributeUtil.IsPrime(span);
                rowLine.Composite = new DGCellModel() { Text = IsPrimary ? "" : "合", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Primary = new DGCellModel() { Text = IsPrimary ? "质" : "", ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                int remain = (span % 3);
                rowLine.Remain0 = new DGCellModel() { Text = (remain == 0 ? "0" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Remain1 = new DGCellModel() { Text = (remain == 1 ? "1" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Remain2 = new DGCellModel() { Text = (remain == 2 ? "2" : ""), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                int wxIndex = AttributeUtil.IndexOf5Xing(span);
                rowLine.WXJin = new DGCellModel() { Text = (wxIndex == 1 ? "金" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXMu = new DGCellModel() { Text = (wxIndex == 2 ? "木" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXShui = new DGCellModel() { Text = (wxIndex == 3 ? "水" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXHuo = new DGCellModel() { Text = (wxIndex == 4 ? "火" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.WXTu = new DGCellModel() { Text = (wxIndex == 5 ? "土" : ""), ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                _viewData.Add(rowLine);

                preRow = rowLine;
            }
        }

        private static void AdjustLine(ref double startX, ref double startY, ref double endX, ref double endY)
        {
            double length = Math.Sqrt(Math.Pow(endX - startX, 2) + 625);
            double offsetX = 10 * (endX - startX) / length;
            double offsetY = 250 / length;

            startX += offsetX;
            startY += offsetY;
            endX -= offsetX;
            endY -= offsetY;
        }
    }

    class DGRedGeneralViewModel : DGViewModelBase
    {
        private ObservableCollection<DGRedGeneralRowModel> _viewData = null;

        private static Color c_HighlightBackColor = ColorPicker.Red;
        private static Color c_LightForeColor = ColorPicker.White;
        private static Color c_DimForeColor = ColorPicker.DarkGray;
        private static Color c_PrimaryBackColor = ColorPicker.White;
        private static Color c_SecondaryBackColor = ColorPicker.LightGoldenrodYellow;

        public override object GetViewData()
        {
            return _viewData;
        }

        public override void SetOptions(StatusOptions op)
        {
            if (_viewData == null)
                return;

            DGRedGeneralRowModel preRow = null;

            foreach (DGRedGeneralRowModel row in _viewData)
            {
                int visibleColCount = 0;
                string dispText = "";
                int highlightColIndex = -1;                

                switch (op.ViewOption)
                {
                    case StatusViewOption.RedSumDetail:
                        {
                            visibleColCount = 9;
                            dispText = row.Sum.Text;
                            highlightColIndex = AttributeUtil.IndexOfSum(row.Sum.Traits);
                            break;
                        }
                    case StatusViewOption.RedContinuityDetail:
                        {
                            visibleColCount = 6;
                            dispText = row.Continuity.Text;
                            highlightColIndex = row.Continuity.Traits;
                            break;
                        }
                    case StatusViewOption.RedEvenOddDetail:
                        {
                            visibleColCount = 7;
                            dispText = row.Odd.Text + "-" + row.Even.Text;
                            highlightColIndex = row.Odd.Traits;
                            break;
                        }
                    case StatusViewOption.RedPrimaryCompositeDetail:
                        {
                            visibleColCount = 7;
                            dispText = row.Primary.Text + "-" + row.Composite.Text;
                            highlightColIndex = row.Primary.Traits;
                            break;
                        }
                    case StatusViewOption.RedBigSmallDetail:
                        {
                            visibleColCount = 7;
                            dispText = row.Big.Text + "-" + row.Small.Text;
                            highlightColIndex = row.Big.Traits;
                            break;
                        }
                    case StatusViewOption.RedRemain0Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Remain0.Text;
                            highlightColIndex = row.Remain0.Traits;
                            break;
                        }
                    case StatusViewOption.RedRemain1Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Remain1.Text;
                            highlightColIndex = row.Remain1.Traits;
                            break;
                        }
                    case StatusViewOption.RedRemain2Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Remain2.Text;
                            highlightColIndex = row.Remain2.Traits;
                            break;
                        }
                    case StatusViewOption.RedDiv1Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Div1.Text;
                            highlightColIndex = row.Div1.Traits;
                            break;
                        }
                    case StatusViewOption.RedDiv2Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Div2.Text;
                            highlightColIndex = row.Div2.Traits;
                            break;
                        }
                    case StatusViewOption.RedDiv3Detail:
                        {
                            visibleColCount = 7;
                            dispText = row.Div3.Text;
                            highlightColIndex = row.Div3.Traits;
                            break;
                        }
                }

                for (int i = 0; i < 9; ++i)
                {
                    if (i < visibleColCount)
                    {
                        row.Extends[i].Text = dispText;
                        row.Extends[i].ForeColor = c_LightForeColor;
                        row.Extends[i].BackColor = (highlightColIndex != i) ? c_PrimaryBackColor : c_HighlightBackColor;
                        row.Extends[i].IsVisible = true;
                    }
                    else
                    {
                        row.Extends[i].IsVisible = false;
                    }
                }

                if (visibleColCount > 0)
                {
                    row.Line1StartY = 0;
                    row.Line1EndX = 50 * highlightColIndex + 25;
                    row.Line1EndY = 12.5;

                    row.Line2EndY = 25;
                    row.Line2StartY = 12.5;
                    row.Line2StartX = 50 * highlightColIndex + 25;

                    row.HasLine1 = Visibility.Collapsed;
                    row.HasLine2 = Visibility.Collapsed;

                    if (preRow != null)
                    {
                        // line 2 for previous row.                        
                        preRow.Line2EndX = (preRow.Line2StartX + row.Line1EndX) / 2;

                        // Line1 for this row.                        
                        row.Line1StartX = preRow.Line2EndX;

                        // Adjust line
                        double startX = preRow.Line2StartX;
                        double startY = preRow.Line2StartY;
                        double endX = row.Line1EndX;
                        double endY = row.Line1EndY;

                        if (AdjustLine(ref startX, ref startY, ref endX, ref endY))
                        {
                            preRow.Line2StartX = startX;
                            preRow.Line2StartY = startY;
                            row.Line1EndX = endX;
                            row.Line1EndY = endY;

                            preRow.HasLine2 = Visibility.Visible;
                            row.HasLine1 = Visibility.Visible;
                        }
                    }

                    preRow = row;
                }
                else
                {
                    row.HasLine1 = Visibility.Collapsed;
                    row.HasLine2 = Visibility.Collapsed;
                }
            }
        }

        public override void Init(List<Lottery> lotteries)
        {
            _viewData = new ObservableCollection<DGRedGeneralRowModel>();

            foreach (Lottery lot in lotteries)
            {
                DGRedGeneralRowModel rowLine = new DGRedGeneralRowModel();

                for (int i = 0; i < 9; ++i)
                {
                    rowLine.Extends[i] = new DGCellModel() { IsVisible = false, Text="" };
                }

                // Add properties.
                int sum = lot.Scheme.Sum;
                rowLine.Sum = new DGCellModel() { Text = sum.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = sum };

                int conti = lot.Scheme.Continuity;
                rowLine.Continuity = new DGCellModel() { Text = conti.ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = conti };

                int even = lot.Scheme.EvenNumCount;
                rowLine.Odd = new DGCellModel() { Text = (6 - even).ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = 6 - even };
                rowLine.Even = new DGCellModel() { Text = even.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = even };

                int small = lot.Scheme.SmallNumCount;
                rowLine.Big = new DGCellModel() { Text = (6 - small).ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = 6 - small };
                rowLine.Small = new DGCellModel() { Text = small.ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = small };

                int prim = lot.Scheme.PrimeNumCount;
                rowLine.Primary = new DGCellModel() { Text = prim.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = prim };
                rowLine.Composite = new DGCellModel() { Text = (6 - prim).ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = 6 - prim };

                int remain0 = 0, remain1 = 0, remain2 = 0;
                int remainBy3Index = lot.Scheme.GetRemainBy3(ref remain0, ref remain1, ref remain2);
                rowLine.Remain0 = new DGCellModel() { Text = remain0.ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = remain0 };
                rowLine.Remain1 = new DGCellModel() { Text = remain1.ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = remain1 };
                rowLine.Remain2 = new DGCellModel() { Text = remain2.ToString(), ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor, Traits = remain2 };

                int smb0 = 0, smb1 = 0, smb2 = 0;
                int divIn3Index = lot.Scheme.GetSmallMiddleBig(ref smb0, ref smb1, ref smb2);
                rowLine.Div1 = new DGCellModel() { Text = smb0.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = smb0 };
                rowLine.Div2 = new DGCellModel() { Text = smb1.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = smb1 };
                rowLine.Div3 = new DGCellModel() { Text = smb2.ToString(), ForeColor = c_DimForeColor, BackColor = c_SecondaryBackColor, Traits = smb2 };

                _viewData.Add(rowLine);
            }
        }

        private static bool AdjustLine(ref double startX, ref double startY, ref double endX, ref double endY)
        {
            if (Math.Abs(startX - endX) <= 50)
            {
                return false;
            }
            else
            {
                double dHDistance = endX - startX;

                double offsetY = 625 / Math.Abs(dHDistance);
                double offsetX = dHDistance > 0 ? 25 : -25;

                startX += offsetX;
                startY += offsetY;
                endX -= offsetX;
                endY -= offsetY;

                return true;
            }
        }
    }

    class DGRedPositionViewModel : DGViewModelBase
    {
        private ObservableCollection<DGRedPositionRowModel> _viewData = null;

        private static Color c_BallColor = ColorPicker.Red;
        private static Color c_LightForeColor = ColorPicker.White;
        private static Color c_DarkForeColor = ColorPicker.LightGray;
        private static Color c_DimForeColor = ColorPicker.DarkGray;
        private static Color c_PrimaryBackColor = ColorPicker.White;
        private static Color c_SecondaryBackColor = ColorPicker.LightGoldenrodYellow;
        private static Color c_PropertyAltBackColor = ColorPicker.Azure;

        public override object GetViewData()
        {
            return _viewData;
        }

        public override void SetOptions(StatusOptions op)
        {
            if (_viewData == null)
                return;

            int position = RedPositionFromType(op.SubCategory);

            DGRedPositionRowModel preRow = null;

            int groupCount = GetGroupCount();

            foreach (DGRedPositionRowModel row in _viewData)
            {
                // Get the number from data according to the curren toption.
                int num = row.Data.GetRedNums()[position - 1];
                int colIndex = NumberInColumn(position, num);

                for (int i = 0; i < groupCount; ++i)
                {
                    if (colIndex == i)
                    {
                        row.Cells[i].Text = num.ToString().PadLeft(2, '0');
                        row.Cells[i].ShowAsBall = true;

                        // update lines.
                        row.Line1StartY = 0;
                        row.Line2EndY = 25;
                        row.Line1EndX = 25 * i + 12.5;
                        row.Line1EndY = 12.5;
                        row.Line2StartX = 25 * i + 12.5;
                        row.Line2StartY = 12.5;

                        row.HasLine1 = Visibility.Collapsed;
                        row.HasLine2 = Visibility.Collapsed;

                        if (preRow != null)
                        {
                            // line 2 for previous row.
                            preRow.HasLine2 = Visibility.Visible;
                            preRow.Line2EndX = (preRow.Line2StartX + row.Line1EndX) / 2;

                            // Line1 for this row.
                            row.HasLine1 = Visibility.Visible;
                            row.Line1StartX = preRow.Line2EndX;

                            // Adjust line
                            double startX = preRow.Line2StartX;
                            double startY = preRow.Line2StartY;
                            double endX = row.Line1EndX;
                            double endY = row.Line1EndY;

                            AdjustLine(ref startX, ref startY, ref endX, ref endY);

                            preRow.Line2StartX = startX;
                            preRow.Line2StartY = startY;
                            row.Line1EndX = endX;
                            row.Line1EndY = endY;
                        }
                    }
                    else
                    {
                        row.Cells[i].Text = "";
                        row.Cells[i].ShowAsBall = false;
                    }
                }

                // Add properties.
                bool IsEven = (num % 2 == 0);
                row.Odd.Text = IsEven ? "" : "奇";
                row.Even.Text = IsEven ? "偶" : "";

                bool IsPrimary = AttributeUtil.IsPrime(num);
                row.Composite.Text = IsPrimary ? "" : "合";
                row.Primary.Text = IsPrimary ? "质" : "";

                int remain = (num % 3);
                row.Remain0.Text = (remain == 0 ? "0" : "");
                row.Remain1.Text = (remain == 1 ? "1" : "");
                row.Remain2.Text = (remain == 2 ? "2" : "");

                int wxIndex = AttributeUtil.IndexOf5Xing(num);
                row.WXJin.Text = (wxIndex == 1 ? "金" : "");
                row.WXMu.Text = (wxIndex == 2 ? "木" : "");
                row.WXShui.Text = (wxIndex == 3 ? "水" : "");
                row.WXHuo.Text = (wxIndex == 4 ? "火" : "");
                row.WXTu.Text = (wxIndex == 5 ? "土" : "");

                preRow = row;
            }
        }

        public override void Init(List<Lottery> lotteries)
        {
            _viewData = new ObservableCollection<DGRedPositionRowModel>();
            int groupCount = GetGroupCount();

            foreach (Lottery lot in lotteries)
            {
                DGRedPositionRowModel rowLine = new DGRedPositionRowModel() { Data = lot.Scheme };
                rowLine.Cells = new ObservableCollection<DGCellModel>(new DGCellModel[groupCount]);

                for (int i = 0; i < groupCount; ++i)
                {
                    rowLine.Cells[i] = new DGCellModel();
                    rowLine.Cells[i].Text = "";
                    rowLine.Cells[i].ForeColor = c_LightForeColor;
                    rowLine.Cells[i].BackColor = c_PrimaryBackColor;
                    rowLine.Cells[i].BallColor = c_BallColor;
                }

                // Add properties.
                rowLine.Odd = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Even = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                rowLine.Composite = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.Primary = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                rowLine.Remain0 = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Remain1 = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };
                rowLine.Remain2 = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PropertyAltBackColor };

                rowLine.WXJin = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.WXMu = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.WXShui = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.WXHuo = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };
                rowLine.WXTu = new DGCellModel() { ForeColor = c_DimForeColor, BackColor = c_PrimaryBackColor };

                _viewData.Add(rowLine);
            }
        }

        public static int RedPositionFromType(StatusSubCategory cat)
        {
            int pos = -1;
            switch (cat)
            {
                case StatusSubCategory.RedPosition1:
                    pos = 1;
                    break;
                case StatusSubCategory.RedPosition2:
                    pos = 2;
                    break;
                case StatusSubCategory.RedPosition3:
                    pos = 3;
                    break;
                case StatusSubCategory.RedPosition4:
                    pos = 4;
                    break;
                case StatusSubCategory.RedPosition5:
                    pos = 5;
                    break;
                case StatusSubCategory.RedPosition6:
                    pos = 6;
                    break;
            }

            return pos;
        }

        public static int NumberInColumn(int pos, int num)
        {
            int index = 0;
            switch (pos)
            {
                case 1:
                    index = num > 17 ? 17 : num - 1;
                    break;
                case 2:
                    index = num > 18 ? 17 : num - 2;
                    break;
                case 3:
                    index = num > 22 ? 17 : (num < 7) ? 0 : num - 6;
                    break;
                case 4:
                    index = num > 27 ? 17 : (num < 12) ? 0 : num - 11;
                    break;
                case 5:
                    index = num < 16 ? 0 : num - 15;
                    break;
                case 6:
                    index = num < 17 ? 0 : num - 16;
                    break;
            }

            return index;
        }

        public static int GetGroupCount()
        {
            return 18; // should match with the following group string count.
        }

        public static string[] GetRedGroupInfo(int pos)
        {
            if (pos == 1)
            {
                return new string[] { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18 - 28" };
            }
            else if (pos == 2)
            {
                return new string[] { "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19 - 29" };
            }
            else if (pos == 3)
            {
                return new string[] { "03 - 06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23 - 30" };
            }
            else if (pos == 4)
            {
                return new string[] { "04 - 11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28 - 31" };
            }
            else if (pos == 5)
            {
                return new string[] { "05 - 15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32" };
            }
            else // (pos == 6)
            {
                return new string[] { "06 - 16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32", "33" };
            }
        }

        private static void AdjustLine(ref double startX, ref double startY, ref double endX, ref double endY)
        {
            double length = Math.Sqrt(Math.Pow(endX - startX, 2) + 625);
            double offsetX = 10 * (endX - startX) / length;
            double offsetY = 250 / length;

            startX += offsetX;
            startY += offsetY;
            endX -= offsetX;
            endY -= offsetY;
        }
    }

    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>

    enum RedDivsionTrait
    {
        Hitted = 1,
        HConnected = 2,
        VConnected = 4,
        OConnected = 8,
        OddConnected = 16,
        EvenConnected = 32
    }

    class ConnectionPattern
    {
        public StatusViewOption Option = StatusViewOption.RedVerticalConnection;
        public int Flag = 0;
        private DGCellModel[,] _testData = null;
        private bool[,] _visitedFlags = null;
        private int _rowCount = 0;

        public ConnectionPattern(DGCellModel[,] test)
        {
            _testData = test;
            _rowCount = _testData.Length / 33;
            
            ResetFlags();
        }

        public void ResetFlags()
        {
            _visitedFlags = new bool[33, _rowCount];
        }

        public void Match(int row, int col)
        {
            int number = col + 1;
            DGCellModel item = _testData[col, row];

            // has been visited?
            if (item == null || _visitedFlags[col, row] || !CheckStart(number))
                return;

            List<DGCellModel> matched = new List<DGCellModel>();
            matched.Add(item);
            _visitedFlags[col, row] = true;

            // Search for next.
            while (item != null)
            {
                if (Next(ref row, ref col, ref item) && item != null)
                {
                    matched.Add(item);
                    _visitedFlags[col, row] = true;
                }
            }

            if (matched.Count >= 3)
            {
                // mark all of them matched.
                foreach (DGCellModel vm in matched)
                {
                    if (Option == StatusViewOption.RedHorizantalConnection)
                    {
                        vm.Traits |= (int)RedDivsionTrait.HConnected;
                    }
                    else if (Option == StatusViewOption.RedVerticalConnection)
                    {
                        vm.Traits |= (int)RedDivsionTrait.VConnected;
                    }
                    else if (Option == StatusViewOption.RedObliqueConnection)
                    {
                        vm.Traits |= (int)RedDivsionTrait.OConnected;
                    }
                    else if (Option == StatusViewOption.RedOddConnection)
                    {
                        vm.Traits |= (int)RedDivsionTrait.OddConnected;
                    }
                    else if (Option == StatusViewOption.RedEvenConnection)
                    {
                        vm.Traits |= (int)RedDivsionTrait.EvenConnected;
                    }
                }
            }
        }

        private bool CheckStart(int number)
        {
            if (Option == StatusViewOption.RedHorizantalConnection)
            {
                return number <= 31;
            }
            else if (Option == StatusViewOption.RedVerticalConnection)
            {
                return true;
            }
            else if (Option == StatusViewOption.RedObliqueConnection)
            {
                return Flag == 0 ? number >= 3 : number <= 31;
            }
            else if (Option == StatusViewOption.RedOddConnection)
            {
                return (number % 2 == 1) && (Flag == 0 ? number >= 5 : number <= 29);
            }
            else if (Option == StatusViewOption.RedEvenConnection)
            {
                return (number % 2 == 0) && (Flag == 0 ? number >= 6 : number <= 28);
            }

            return false;
        }

        private bool Next(ref int row, ref int col, ref DGCellModel vm)
        {
            if (Option == StatusViewOption.RedHorizantalConnection)
            {
                if (col < 32)
                {
                    DGCellModel next = _testData[col + 1, row];
                    if (next != null)
                    {
                        ++ col;
                        vm = next;
                        return true;
                    }
                }
            }
            else if (Option == StatusViewOption.RedVerticalConnection)
            {
                if (row < _rowCount - 1)
                {
                    DGCellModel next = _testData[col, row + 1];
                    if (next != null)
                    {
                        ++row;
                        vm = next;
                        return true;
                    }
                }
            }
            else if (Option == StatusViewOption.RedObliqueConnection)
            {
                if (row < _rowCount - 1)
                {
                    if (Flag == 0 && col > 0) // left
                    {
                        DGCellModel next = _testData[col - 1, row + 1];
                        if (next != null)
                        {
                            ++row;
                            --col;
                            vm = next;
                            return true;
                        }
                    }

                    if (Flag == 1 && col < 32) // right
                    {
                        DGCellModel next = _testData[col + 1, row + 1];
                        if (next != null)
                        {
                            ++row;
                            ++col;
                            vm = next;
                            return true;
                        }
                    }
                }
            }
            else if (Option == StatusViewOption.RedOddConnection || Option == StatusViewOption.RedEvenConnection)
            {
                if (row < _rowCount - 1)
                {
                    if (Flag == 0 && col > 1) // left
                    {
                        DGCellModel next = _testData[col - 2, row + 1];
                        if (next != null)
                        {
                            ++row;
                            col -= 2;
                            vm = next;
                            return true;
                        }
                    }

                    if (Flag == 1 && col < 31) // right
                    {
                        DGCellModel next = _testData[col + 2, row + 1];
                        if (next != null)
                        {
                            ++row;
                            col += 2;
                            vm = next;
                            return true;
                        }
                    }
                }
            }

            vm = null;
            return false;
        }
    }

    class ConnectionDetector
    {
        private List<ConnectionPattern> _patterns = new List<ConnectionPattern>();
        private DGCellModel[,] _testData = null;
        private int _rowCount = 0;

        public ConnectionDetector(int rowCount)
        {
            _testData = new DGCellModel[33, rowCount];
            _rowCount = rowCount;
        }

        public void SetTest(int col, int row, DGCellModel model)
        {
            _testData[col, row] = model;
        }

        public void Detect()
        {
            if (_patterns.Count == 0)
            {
                // init the patterns
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedVerticalConnection });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedHorizantalConnection });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedObliqueConnection, Flag = 0 });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedObliqueConnection, Flag = 1 });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedOddConnection, Flag = 0 });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedOddConnection, Flag = 1 });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedEvenConnection, Flag = 0 });
                _patterns.Add(new ConnectionPattern(_testData) { Option = StatusViewOption.RedEvenConnection, Flag = 1 });
            }

            foreach (ConnectionPattern pattern in _patterns)
            {
                for (int row = 0; row < _rowCount; ++row)
                {
                    for (int col = 0; col < 33; ++col)
                    {
                        pattern.Match(row, col);
                    }
                }

                // Clear visited flags for next usage.
                pattern.ResetFlags();
            }
        }
    }
}
