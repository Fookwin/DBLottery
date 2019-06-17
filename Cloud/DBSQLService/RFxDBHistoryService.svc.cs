using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using DBSQLService.Data;
using System.ServiceModel.Web;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RFxDBHistoryService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RFxDBHistoryService.svc or RFxDBHistoryService.svc.cs at the Solution Explorer and start debugging.
    public class RFxDBHistoryService : IRFxDBHistoryService
    {

        GetLotteriesResult IRFxDBHistoryService.GetLotteries(int tailIndex, int pageSize)
        {
            List<Basic> basicList = null;
            List<Detail> detailList = null;
            List<Omission> omissionList = null;
            List<Attribute> attributeList = null;
            DBSQLClient.Instance().GetRecordList(out basicList, out detailList, out omissionList, out attributeList);

            // get the required sub set of the data
            if (tailIndex > basicList.Count() || pageSize <= 0)
            {
                throw new WebFaultException(HttpStatusCode.BadRequest);
            }

            // get the range of the required data
            tailIndex = tailIndex == 0 ? basicList.Count() : tailIndex;
            pageSize = Math.Min(tailIndex, pageSize);

            var requiredBaseList = basicList.GetRange(tailIndex - pageSize, pageSize);
            var requiredDetailList = detailList.GetRange(tailIndex - pageSize, pageSize);

            // outputing
            var lottos = new List<DataModel.DBLotteryModel>();
            for (int i = pageSize - 1; i >= 0; --i)
            {
                lottos.Add(DataUtil.BuildLotteryModel(requiredBaseList[i], requiredDetailList[i]));
            }

            return new GetLotteriesResult()
            {
                NextIndex = tailIndex == pageSize ? -1 : tailIndex - pageSize,
                Lotteries = lottos
            }; ;
        }
    }
}
