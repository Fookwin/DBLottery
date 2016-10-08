using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Util;

namespace LuckyBallsData.UserData
{
    public class Record
    {
        public string DeviceID
        {
            get;
            set;
        }

        public int[] Bonus
        {
            get;
            set;
        }

        public int Cost
        {
            get;
            set;
        }

        public int Prize
        {
            get;
            set;
        }

        public int Issue
        {
            get;
            set;
        }

        public string GetBonus()
        {
            string bonus = "";
            foreach (int count in Bonus)
            {
                bonus += count.ToString() + " ";
            }

            return bonus.Trim();
        }

        public void Read(DBXmlNode node)
        {
            Issue = Convert.ToInt32(node.GetAttribute("IS"));
            Cost = Convert.ToInt32(node.GetAttribute("CT"));
            Prize = Convert.ToInt32(node.GetAttribute("PZ"));
            DeviceID = node.GetAttribute("ID");

            Bonus = new int[6] { 0,0,0,0,0,0 };
            string bonus = node.GetAttribute("BN");
            string[] subStrings = bonus.Split(' ');
            for (int index = 0; index < subStrings.Count(); index++)
            {
                Bonus[index] = Convert.ToInt32(subStrings[index]);
            }
        }

        public void Write(DBXmlNode node)
        {
            node.SetAttribute("IS", Issue.ToString());
            node.SetAttribute("CT", Cost.ToString());
            node.SetAttribute("PZ", Prize.ToString());
            node.SetAttribute("ID", DeviceID);

            string bonus = GetBonus();
            node.SetAttribute("BN", bonus);
        }
    }
}