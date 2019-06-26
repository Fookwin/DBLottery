using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using DBSQLService.Data;
using System.ServiceModel.Web;
using DataModel;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RFxDBHistoryService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RFxDBHistoryService.svc or RFxDBHistoryService.svc.cs at the Solution Explorer and start debugging.
    public class RFxDBHistoryService : IRFxDBHistoryService
    {
        GetLotteriesResult IRFxDBHistoryService.GetLotteryBasicList()
        {
            List<LottoBasic> basicList = null;
            DBSQLClient.Instance().GetLottoBasicList(out basicList);

            var requiredBaseList = basicList;

            // outputing
            var lottos = new List<DataModel.DBLotteryBasicModel>();
            for (int i = basicList.Count - 1; i >= 0; --i)
            {
                lottos.Add(DataUtil.BuildLotteryBasicModel(requiredBaseList[i]));
            }

            return new GetLotteriesResult()
            {
                Lotteries = lottos
            };
        }

        DBLotteryModel IRFxDBHistoryService.GetLotteryDetail(int issue)
        {
            Basic basic = null;
            Detail detail = null;
            Omission omission = null;
            Attribute attribute = null;
            if (!DBSQLClient.Instance().GetRecord(issue, out basic, out detail, out omission, out attribute))
                throw new Exception("Invalid issue number.");

            // outputing
            return DataUtil.BuildLotteryModel(basic, detail);
        }
    }
}
