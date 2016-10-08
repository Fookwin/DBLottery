using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Selection;
using LuckyBallsData.Statistics;

namespace LuckyBallsData.Selection
{
    public class Purchase
    {
        private List<SchemeSelector> _selectors = new List<SchemeSelector>();
        private List<Constraint> _filters = new List<Constraint>();
        private List<Scheme> _selection = new List<Scheme>();

        public List<SchemeSelector> Selectors
        {
            get
            {
                return _selectors;
            }
            set
            {
                _selectors = value;
            }
        }

        public List<Constraint> Filters
        {
            get
            {
                return _filters;
            }
            set
            {
                _filters = value;
            }
        }

        public List<Scheme> Selection
        {
            get
            {
                return _selection;
            }
            set
            {
                _selection = value;
            }
        }

        public Purchase Clone()
        {
            Purchase copy = new Purchase();

            copy.Selectors = new List<SchemeSelector>();
            foreach (SchemeSelector selector in Selectors)
            {
                copy.Selectors.Add(selector.Clone());
            }

            copy.Filters = new List<Constraint>();
            foreach (Constraint con in Filters)
            {
                copy.Filters.Add(con.Clone());
            }

            copy.Selection = new List<Scheme>();
            foreach (Scheme item in Selection)
            {
                copy.Selection.Add(new Scheme(item));
            }

            return copy;
        }
    }

    public class PurchaseBucket
    {
        private int _issue = -1;
        private List<Purchase> _orders = new List<Purchase>();

        public List<Purchase>  Orders
        {
            get { return _orders; }
        }

        public int Issue
        {
            get
            {
                return _issue;
            }

            set
            {
                _issue = value;
            }
        }
    }    
}
