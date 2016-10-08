using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IUseControlService
    {
        // User management
        //

        [OperationContract]
        void UserLogin(string devId, int platform, int clientVersion, string info);

        // Software version Control
        //
        [OperationContract]
        void GetLatestSoftwareVersion(int platform, ref int version, ref bool forceUpgradingRequired);

        [OperationContract]
        string GetReleaseNotes(int platform, int clientVersion);

        // User feedback and win record
        [OperationContract]
        void PostFeedback(string feedback);

        [OperationContract]
        void PostRecord(string record);
    }
}
