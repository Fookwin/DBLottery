﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataModel;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "RFxDBManageService" in both code and config file together.
    [ServiceContract]
    public interface IRFxDBManageService
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            UriTemplate = "/PushNotification/?platform={platform}&message={message}&devices={devices}",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        void PushNotification(int platform, string message, string devices);

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/SearchReleaseFromWeb", 
            BodyStyle = WebMessageBodyStyle.Bare, 
            RequestFormat = WebMessageFormat.Json, 
            ResponseFormat = WebMessageFormat.Json)]
        DBReleaseModel SearchReleaseFromWeb();

        [OperationContract]
        [WebInvoke(Method = "GET",
            UriTemplate = "/GetNextReleaseInfo",
            BodyStyle = WebMessageBodyStyle.Bare,
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        string GetNextReleaseInfo();

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
