using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace DBSQLService
{
    [DataContract]
    public class GetLotteriesResult
    {
        // -1 means has been reached the end yet.
        [DataMember]
        public int NextIndex
        {
            get;
            set;
        }

        [DataMember]
        public List<DBLotteryModel> Lotteries
        {
            get;
            set;
        }
    };

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRFxDBHistoryService" in both code and config file together.
    [ServiceContract]
    public interface IRFxDBHistoryService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                    UriTemplate = "/Lotteries/?tail={tailIndex}&page={pageSize}",
                    BodyStyle = WebMessageBodyStyle.Bare,
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        GetLotteriesResult GetLotteries(int tailIndex = 0/*default the last issue*/, int pageSize = 30);
    }
}
