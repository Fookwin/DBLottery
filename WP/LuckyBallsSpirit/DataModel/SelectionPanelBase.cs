using System;
using System.Collections.Generic;
using System.Linq;
using LuckyBallsData.Selection;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace LuckyBallsSpirit.DataModel
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
