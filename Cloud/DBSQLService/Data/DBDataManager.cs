using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuckyBallsData.Selection;
using LuckyBallsData.Util;

namespace DBSQLService.Data
{
    public class DBDataManager
    {
        private static DBDataManager _mgr = new DBDataManager();

        public static DBDataManager Instance()
        {
            return _mgr;
        }

        public SchemeAttributes ReadAttributes(int issue)
        {
            // get the attributes file for this issue.
            string attriString = DBCloudStorageClient.Instance().ReadBlobAsString("data-attributes", issue.ToString() + ".xml");
            if (string.IsNullOrEmpty(attriString))
                return null;

            DBXmlDocument xml = new DBXmlDocument();
            xml.Load(attriString);

            SchemeAttributes attriSet = new SchemeAttributes();
            attriSet.ReadValueFromXml(xml.Root());

            return attriSet;
        }
    }
}