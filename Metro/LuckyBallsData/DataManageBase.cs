using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;

namespace LuckyBallsData
{
    public abstract class DataManageBase
    {
        protected static DataManageBase _Instance = null;
        protected History _History = null;

        public static DataManageBase Instance()
        {
            if (_Instance == null)
                throw new Exception("Data Manager has not been instantiated yet.");

            return _Instance;
        }

        public History History
        {
            get
            {
                return _History;
            }
        }
    }
}
