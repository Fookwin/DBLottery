using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using LuckyBallSpirit.DataModel;

namespace LuckyBallSpirit.ViewModel
{
    public class SimpleNumberBallViewModel
    {
        public string Number
        {
            get;
            set;
        }

        public Color StrokeColor
        {
            get;
            set;
        }

        public Color FillColor
        {
            get;
            set;
        }

    }

    public enum NumStatusType
    {
        HotNumber = 0,
        CoolNumber = 1,
        RecommendedNumber = 2,
        KilledNumber = 3
    }

    public class ConvertStatusToBallGroupHelper
    {
        public static List<SimpleNumberBallViewModel> GetNumbers(NumberStateInfo[] data, bool bRed, NumStatusType sType)
        {
            List<SimpleNumberBallViewModel> output = new List<SimpleNumberBallViewModel>();

            foreach (NumberStateInfo num in data)
            {
                //Color fillColor = Colors.White;

                if (sType == NumStatusType.HotNumber && num.State.Temperature >= (bRed ? 4 : 2))
                {
                    //fillColor = Colors.White;                    
                }
                else if (sType == NumStatusType.CoolNumber && num.State.Temperature == 0)
                {
                    //fillColor = Colors.White;
                }
                else if (sType == NumStatusType.RecommendedNumber && num.Included)
                {
                    //fillColor = Colors.White;
                }
                else if (sType == NumStatusType.KilledNumber && num.Excluded)
                {
                    //fillColor = Colors.White;
                }
                else
                {
                    continue;
                }

                //fillColor.A = 200;

                // Add item.
                SimpleNumberBallViewModel item = new SimpleNumberBallViewModel();
                item.Number = num.Number.ToString().PadLeft(2, '0');                
                item.StrokeColor = Colors.White;
                item.FillColor = bRed ? Colors.Red : Colors.Blue;

                output.Add(item);
            }

            return output;
        }
    }
}
