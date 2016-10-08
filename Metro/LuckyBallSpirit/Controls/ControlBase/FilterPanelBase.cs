using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;

namespace LuckyBallSpirit.Controls
{
    public interface FilterPanelBase
    {
        Constraint Constraint
        {
            get;
        }

        void SetConstraint(Constraint _constr);
    }
}
