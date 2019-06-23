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
        [DataMember]
        public List<DBLotteryBasicModel> Lotteries
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
                    UriTemplate = "/Lotteries",
                    BodyStyle = WebMessageBodyStyle.Bare,
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        GetLotteriesResult GetLotteryBasicList();
    }
}
