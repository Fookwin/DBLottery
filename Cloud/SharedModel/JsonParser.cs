using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBallsData.Util
{
    public static class JsonParser
    {
        #region JsonParser Methods

        public static T Parse<T>(string jsonString)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string Stringify(object jsonObject)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                new System.Runtime.Serialization.Json.DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        #endregion
    }
}
