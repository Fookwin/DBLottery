using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuckyBallsData.Util;

namespace LuckyBallsData.UserData
{
    public class Feedback
    {
        public int Plateform
        {
            get;
            set;
        }

        public string DeviceID
        {
            get;
            set;
        }

        public int LocalVersion
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Phone
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public void Read(DBXmlNode node)
        {
            Plateform = Convert.ToInt32(node.GetAttribute("PL"));
            LocalVersion = Convert.ToInt32(node.GetAttribute("VR"));
            DeviceID = node.GetAttribute("ID");
            Name = node.GetAttribute("NM");
            Email = node.GetAttribute("EM");
            Phone = node.GetAttribute("PH");
            Content = node.GetAttribute("CT");
        }

        public void Write(DBXmlNode node)
        {
            node.SetAttribute("PL", Plateform.ToString());
            node.SetAttribute("VR", LocalVersion.ToString());
            node.SetAttribute("ID", DeviceID);
            node.SetAttribute("NM", Name);
            node.SetAttribute("EM", Email);
            node.SetAttribute("PH", Phone);
            node.SetAttribute("CT", Content);
        }
    }
}