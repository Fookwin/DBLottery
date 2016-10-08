using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using LuckyBallsData.Selection;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace LuckyBallSpirit.Controls
{
    public interface ISelectionPanelBase
    {
        SchemeSelector Selector
        {
            get;
        }

        void SetSelector(SchemeSelector _selector);
    }
}
