using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace LuckyBallsSpirit.Controls
{
    public sealed partial class ReleaseTimeCountDownPanel : UserControl
    {
        private DispatcherTimer _timer = null;
        private Int32 _TotalSecond;
        private readonly Int32 _CutoffSecond = 7200;

        public Int32 TotalSecond
        {
            get { return _TotalSecond; }
            set { _TotalSecond = value; }
        }

        public ReleaseTimeCountDownPanel()
        {
            this.InitializeComponent();
        }

        public void Start()
        {
            if (_timer != null && _timer.IsEnabled)
                return;

            if (DataModel.LBDataManager.GetInstance().ReleaseInfo == null)
                return;

            // Get the release date.
            DateTime releaseAt = DataModel.LBDataManager.GetInstance().ReleaseInfo.NextReleaseTime;

            TimeSpan remainTime = releaseAt - DateTime.Now;

            _TotalSecond = (int)remainTime.TotalSeconds;

            if (_TotalSecond > 0)
            {
                // Set timer.
                if (_timer == null)
                {
                    _timer = new DispatcherTimer();
                    _timer.Interval = new TimeSpan(0, 0, 1);
                    _timer.Tick += timer_Tick;
                }
                _timer.Start();
            }
            else
            {
                _TotalSecond = 0;

                string display = "00天 00小时 00分 00秒";
                TB_RemainingTime.Text = display;
                TB_State.Text = "开奖统计中";
            }

            TB_NextIssue.Text = DataModel.LBDataManager.GetInstance().ReleaseInfo.NextIssue.ToString();
        }

        private void timer_Tick(object sender, object e)
        {
            if (TimeCountDown())
            {
                TimeSpan span = TimeSpan.FromSeconds(_TotalSecond);

                string display = span.Days.ToString().PadLeft(2, '0') + "天 ";
                display += span.Hours.ToString().PadLeft(2, '0') + "小时 ";
                display += span.Minutes.ToString().PadLeft(2, '0') + "分 ";
                display += span.Seconds.ToString().PadLeft(2, '0') + "秒";

                TB_RemainingTime.Text = display;

                TB_State.Text = _TotalSecond > _CutoffSecond ? "正在热销中" : "销售已截止";
            }
            else
            {
                _timer.Stop();
                TB_State.Text = "开奖统计中";
            }
        }

        public bool TimeCountDown()
        {
            if (_TotalSecond <= 0)
                return false;
            else
            {
                _TotalSecond--;
                return true;
            }
        }
    }
}
