using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net;
using DBSQLService.Data;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "DBManagement" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select DBManagement.svc or DBManagement.svc.cs at the Solution Explorer and start debugging.
    public class DBManagement : IDBManagement
    {
        public void PushNotification(int platform, string message, string devices)
        {
            if (devices != "")
            {
                // TODO: we will notify the users on the specficied devices.
            }

            DBNotification.Instance().PushNotification((PlatformIndex)platform, message, new List<string>());
        }
    }
}
