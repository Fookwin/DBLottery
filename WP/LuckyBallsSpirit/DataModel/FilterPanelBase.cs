using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using LuckyBallsData.Selection;

namespace LuckyBallsSpirit.DataModel
{
    public interface FilterPanelBase
    {
        Constraint Constraint
        {
            get;
        }

        void SetConstraint(Constraint _constr);

        void OnNavigatedTo(NavigationContext context);
    }
}
