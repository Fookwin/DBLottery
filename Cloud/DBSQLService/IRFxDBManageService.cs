using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataModel;

namespace DBSQLService
{
    [DataContract]
    public class MessagePocket
    {
        [DataMember]
        public int[] Platforms
        {
            get;
            set;
        }

        [DataMember]
        public string Message
        {
            get;
            set;
        }
    };

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "RFxDBManageService" in both code and config file together.
    [ServiceContract]
    public interface IRFxDBManageService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/PushNotification",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string PushNotification(MessagePocket message);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetLatestRelease",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DBReleaseModel GetLatestRelease();

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/SyncLotteryToOffical/?issue={issue}", 
            BodyStyle = WebMessageBodyStyle.Bare, 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json)]
        DBLotteryModel SyncLotteryToOffical(int issue);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/CalcualateNextReleaseInfo/?currentIssue={currentIssue}&currentDate={currentDate}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        DBReleaseInfoModel CalcualateNextReleaseInfo(int currentIssue, DateTime currentDate);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/NorminateNewRelease",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        bool NorminateNewRelease(DBReleaseModel data);

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/UpdateReleaseDetail",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        bool UpdateReleaseDetail(DBReleaseModel data);

        //[OperationContract]
        //bool UpdateLotteryData(int issue, string data);

        //[OperationContract]
        //bool CommitPendingChanges();
    }
}
