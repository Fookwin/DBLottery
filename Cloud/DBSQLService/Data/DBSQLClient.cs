using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DBSQLService.Data
{
    public class DBSQLClient
    {
        private static DBSQLClient _instance = null;

        private DBHistoryEntity context = new DBHistoryEntity();

        public static DBSQLClient Instance()
        {
            if (_instance == null)
                _instance = new DBSQLClient();

            return _instance;
        }

        private DBSQLClient()
        {
        }

        public int GetRecordCount()
        {
            return context.Basics.Count();
        }

        public void GetRecordList(out List<Basic> basicList, out List<Detail> detailList, 
            out List<Omission> omissionList, out List<Attribute> attributeList)
        {
            basicList = context.Basics.ToList();
            detailList = context.Details.ToList();
            omissionList = context.Omissions.ToList();
            attributeList = context.Attributes.ToList();
        } 

        public bool GetRecord(int issue, out Basic basic, out Detail detail,
            out Omission omission, out Attribute attribute)
        {
            basic = context.Basics.Find(issue);
            detail = context.Details.Find(issue);
            omission = context.Omissions.Find(issue);
            attribute = context.Attributes.Find(issue);
            if (basic == null || detail == null || omission == null || attribute == null)
                return false;

            return true;
        }

        public bool GetRecordBasic(int issue, out Basic basic, out Detail detail)
        {
            basic = context.Basics.Find(issue);
            detail = context.Details.Find(issue);
            if (basic == null || detail == null)
                return false;

            return true;
        }

        public bool GetLastRecord(out Basic basic, out Detail detail, out Omission omission, out Attribute attribute)
        {
            basic = context.Basics.AsEnumerable().Last();
            detail = context.Details.AsEnumerable().Last();
            omission = context.Omissions.AsEnumerable().Last();
            attribute = context.Attributes.AsEnumerable().Last();
            if (basic == null || detail == null || omission == null || attribute == null)
                return false;

            return true;
        }

        public int GetLastIssue()
        {
            Basic basic = context.Basics.AsEnumerable().Last();
            return basic.Issue;
        }

        public DateTime GetLastReleaseTime()
        {
            Detail detail = context.Details.AsEnumerable().Last();
            return detail.Date;
        }
    }
}