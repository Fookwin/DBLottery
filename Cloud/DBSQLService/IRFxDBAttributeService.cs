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
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRFxDBAttributeService" in both code and config file together.
    [ServiceContract]
    public interface IRFxDBAttributeService
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
                    UriTemplate = "/Attribute",
                    BodyStyle = WebMessageBodyStyle.Bare,
                    RequestFormat = WebMessageFormat.Json,
                    ResponseFormat = WebMessageFormat.Json)]
        DBAttributesModel GetLatestAttribute();
    }
}
