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
        public string Title
        {
            get;
            set;
        }

        [DataMember]
        public string Content
        {
            get;
            set;
        }
    };

    [DataContract]
    public class NotificationPocket
    {
        [DataMember]
        public int[] Platforms
        {
            get;
            set;
        }

        [DataMember]
        public MessagePocket Message
        {
            get;
            set;
        }
    };

    [DataContract]
    public class CommitReleaseResultPocket
    {
        [DataMember]
        public int ReturnCode
        {
            get;
            set;
        }

        [DataMember]
        public string ErrorMessage
        {
            get;
            set;
        }

        [DataMember]
        public string Container
        {
            get;
            set;
        }

        [DataMember]
        public List<string> Files
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
        string PushNotification(NotificationPocket notification);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetNotificationTemplates",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        Dictionary<string, MessagePocket> GetNotificationTemplates();

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
            UriTemplate = "/PrecommitRelease",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        CommitReleaseResultPocket PrecommitRelease(DBReleaseModel releaseData);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetPendingActions",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        CommitReleaseResultPocket GetPendingActions();

        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/ExecutePendingActions",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        CommitReleaseResultPocket ExecutePendingActions();
    }
}
