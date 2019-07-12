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
using System.IO;
using LuckyBallsData.Selection;

namespace DBSQLService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "RFxDBAttributeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select RFxDBAttributeService.svc or RFxDBAttributeService.svc.cs at the Solution Explorer and start debugging.
    public class RFxDBAttributeService : IRFxDBAttributeService
    {
        DBAttributesModel IRFxDBAttributeService.GetLatestAttribute()
        {
            // load the template
            string templateXml = DataManager.Get()._GetAttributesTemplate();
            DBXmlDocument template = new DBXmlDocument();
            template.Load(templateXml);

            // get value matrix
            string valuesXml = DataManager.Get()._GetLatestAttributes();
            DBXmlDocument valMatrix = new DBXmlDocument();
            valMatrix.Load(valuesXml);

            SchemeAttributes attriSets = new SchemeAttributes();
            attriSets.ReadFromTemplate(template.Root());
            attriSets.ReadValueFromXml(valMatrix.Root());

            // outputing
            DBAttributesModel output = new DBAttributesModel();

            foreach (KeyValuePair<string, SchemeAttributeCategory> cat in attriSets.Categories)
            {
                DBAttributeCategoryModel dbCat = new DBAttributeCategoryModel() { Name = cat.Value.Name, Display = cat.Value.DisplayName };

                foreach (KeyValuePair<string, SchemeAttribute> attri in cat.Value.Attributes)
                {
                    DBAttributeModel dbAtti = new DBAttributeModel()
                    {
                        Name = attri.Value.Key,
                        Display = attri.Value.DisplayName,
                        Description = attri.Value.Description,
                        HID = attri.Value.HelpID,
                        Region = attri.Value.ValidRegion.ToString()
                    };

                    foreach (SchemeAttributeValueStatus state in attri.Value.ValueStates)
                    {
                        DBAttributeStateModel dbState = new DBAttributeStateModel()
                        {
                            Expression = state.ValueExpression,
                            Region = state.ValueRegion.ToString(),
                            Value = state.ToString()
                        };

                        dbAtti.States.Add(dbState);
                    }

                    dbCat.Attributes.Add(dbAtti);
                }

                output.Categories.Add(dbCat);
            }

            return output;
        }
    }
}
