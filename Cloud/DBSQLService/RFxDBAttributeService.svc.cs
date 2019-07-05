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
using System.Xml;
using Newtonsoft.Json;
using LuckyBallsData.Util;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RFxDBAttributeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RFxDBAttributeService.svc or RFxDBAttributeService.svc.cs at the Solution Explorer and start debugging.
    public class RFxDBAttributeService : IRFxDBAttributeService
    {
        string IRFxDBAttributeService.GetLatestAttribute()
        {
            // load the template
            string templateXml = DataManager.Get()._GetAttributesTemplate();
            DBXmlDocument template = new DBXmlDocument();
            template.Load(templateXml);

            // get value matrix
            string valuesXml = DataManager.Get()._GetLatestAttributes();
            DBXmlDocument valMatrix = new DBXmlDocument();
            valMatrix.Load(valuesXml);

            // combine the data
            var rootT = template.Root();
            var rootV = valMatrix.Root();

            for (int catInx = 0; catInx < rootT.ChildNodes().Count; ++catInx)
            {
                // category level
                var catT = rootT.ChildNodes()[catInx];
                var catV = rootV.ChildNodes()[catInx];

                for (int attriInx = 0; attriInx < catT.ChildNodes().Count; ++attriInx)
                {
                    var attriT = catT.ChildNodes()[attriInx];
                    var attriV = catV.ChildNodes()[attriInx];

                    // copy values
                    for (int stateInx = 0; stateInx < attriT.ChildNodes().Count; ++stateInx)
                    {
                        var stateT = attriT.ChildNodes()[stateInx];
                        var stateV = attriV.ChildNodes()[stateInx];

                        var value = stateV.GetAttribute("Value");
                        stateT.SetAttribute("Value", value);
                    }
                }
            }

            return JsonConvert.SerializeXmlNode(template.NativeDoc(), Newtonsoft.Json.Formatting.None, true);
        }
    }
}
