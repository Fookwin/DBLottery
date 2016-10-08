using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DBSQLService
{
    [ServiceContract]
    public interface ISqlService
    {
        [OperationContract]
        string GetLotteryData(int issue);

        [OperationContract]
        string GetAllLotteries();

        [OperationContract]
        int GetLotteryCount();

        [OperationContract]
        string GetLotteriesByIndex(int index_from, int index_to);

        [OperationContract]
        string GetLotteriesByIssue(int issue_from, int issue_to);

        [OperationContract]
        string GetDataVersion();

        [OperationContract]
        string GetAttributesTemplate();

        [OperationContract]
        string GetLatestAttributes();

        [OperationContract]
        string GetLatestReleaseInfo();

        [OperationContract]
        string GetMatrixTableItem(int candidateCount, int selectCount);

        [OperationContract]
        string GetHelp();

        [OperationContract]
        string GetAllLotteriesBase();

        [OperationContract]
        string GetLotteriesBaseByIndex(int index_from, int index_to);

        [OperationContract]
        string GetLotteriesBaseByIssue(int issue_from, int issue_to);

        #region obsolete
        [OperationContract]
        void GetVersion(ref int version, ref Int64 revisions, ref int latestIssue);

        [OperationContract]
        void RegisterUserChannel(string userId, string channelUri, string platform);

        [OperationContract]
        string[] GetUserChannels(string platform);

        [OperationContract]
        void RememberUserLastLoginDate(string userId);

        [OperationContract]
        void GetLatestSoftwareVersion(ref int version, ref bool schemaChanged);

        [OperationContract]
        string GetReleaseNotes(int fromVersion);
        #endregion
    } 
}
