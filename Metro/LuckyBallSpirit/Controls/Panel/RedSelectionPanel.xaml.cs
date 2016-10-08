using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Windows.UI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;
using LuckyBallSpirit.DataModel;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallSpirit.Controls
{
    public sealed partial class RedSelectionPanel : UserControl
    {
        public event NumButton.SelectionChangedEventHandler SelectionChanged;
        private NumButton.SelectModeEnum selectMode = NumButton.SelectModeEnum.DuplexSelectable;
        private int _numStateType = 0; // 0- omission; 1- temperature; 2 - recommendation

        public RedSelectionPanel()
        {
            this.InitializeComponent();

            this.Loaded += delegate
            {
                UpdateDescrition();
                ConnectSelectChangedEvent(false);
            };
        }

        public int NumDescriptionType
        {
            get
            {
                return _numStateType;
            }
            set
            {
                _numStateType = value;
                UpdateDescrition();
            }
        }

        public NumButton.SelectModeEnum SelectMode
        {
            get
            {
                return selectMode;
            }
            set
            {
                ChangeSelectMode(value);
            }
        }

        private void OnNumSelectChanged(object sender, NumButton.NumStateChangeArg e)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, e);
            }
        }

        private void ChangeSelectMode(NumButton.SelectModeEnum mode)
        {
            if (selectMode == mode)
                return;

            for (int i = 1; i <= 33; ++i)
            {
                // Get control by name.
                string ctrlName = "Red" + i.ToString().PadLeft(2, '0');
                NumButton button = this.FindName(ctrlName) as NumButton;
                if (button != null)
                {
                    button.SelectionChanged -= OnNumSelectChanged;
                    button.SelectMode = mode;
                    button.SelectionChanged += OnNumSelectChanged;
                }
            }
        }

        private void ConnectSelectChangedEvent(bool disconnected)
        {
            for (int i = 1; i <= 33; ++i)
            {
                // Get control by name.
                string ctrlName = "Red" + i.ToString().PadLeft(2, '0');
                NumButton button = this.FindName(ctrlName) as NumButton;
                if (button != null)
                {
                    if (disconnected)
                        button.SelectionChanged -= OnNumSelectChanged;
                    else
                        button.SelectionChanged += OnNumSelectChanged;
                }
            }
        }

        public void SetNumsSelectionState(Set nums, NumButton.SelectStatusEnum state, bool bCleanPrevious)
        {
            for (int i = 1; i <= 33; ++i)
            {
                // Get control by name.
                string ctrlName = "Red" + i.ToString().PadLeft(2, '0');
                NumButton button = this.FindName(ctrlName) as NumButton;
                if (button != null)
                {
                    button.SelectionChanged -= OnNumSelectChanged;

                    if (bCleanPrevious)
                        button.SelectStatus = NumButton.SelectStatusEnum.NotSelected;

                    if (nums.Contains(i))
                        button.SelectStatus = state;

                    button.SelectionChanged += OnNumSelectChanged;
                }
            }
        }

        public void UpdateDescrition()
        {
            LotteryStateInfo info = LBDataManager.GetInstance().GetLatestLotteryStateInfo();            

            for (int i = 1; i <= 33; ++i)
            {
                // Get control by name.
                string ctrlName = "Red" + i.ToString().PadLeft(2, '0');
                NumButton button = this.FindName(ctrlName) as NumButton;
                if (button != null)
                {
                    NumberState state = info.RedsStateInfo[i -1].State;
                    string desc = "";
                    switch (_numStateType)
                    {
                        case 0:// 0- omission;
                            {
                                desc = state.Omission.ToString();
                                button.TipColor = state.Omission > 16 ? Colors.Red : Colors.DarkGray;
                                break;
                            }
                        case 1:// 1- temperature; 
                            {
                                desc = state.Temperature >= 4 ? "热" : (state.Temperature < 2 ? "冷" : "温");
                                button.TipColor = state.Temperature >= 4 ? Colors.Red : (state.Temperature < 2 ? Colors.Blue : Colors.DarkGray);
                                break;
                            }
                        case 2:// 2 - recommendation
                            {
                                desc = info.RedsStateInfo[i - 1].Included ? "胆" : (info.RedsStateInfo[i - 1].Excluded ? "杀" : "");
                                button.TipColor = desc == "胆" ? Colors.Red : Colors.Blue;
                                break;
                            }
                    }

                    button.Description = desc.ToString();
                }
            }
        }
    }
}
