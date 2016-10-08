using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Statistics;
using LuckyBallsData.Util;
using LuckyBallsData.Selection;
using LuckyBallsData.Filters;

namespace LuckyBallsData
{  
    public abstract class DataManageBase
    {
        protected static DataManageBase _Instance = null;
        protected History _History = null;
        protected MatrixTable _matrixTable = null;

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
                if (_History == null)
                    return null;

                return _History;
            }
        }

        public MatrixTable MatrixTable
        {
            get
            {
                return _matrixTable;
            }
        }
    }
}
