using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBallsData.Util
{
    public enum DBResult
    {
        S_TRUE = 0, // successful.
        S_FALSE= 1, // successful with warning.
        F_TIMEOUT = 2,
        F_DATACORRUPTED = 3,
        F_FILEMISSING = 4,
        F_CONNECTION = 5
    }

    public static class ErrorUtil
    {
        public static string GetErrorMessage(DBResult res)
        {
            switch (res)
            {
                case DBResult.F_TIMEOUT: return "连接服务器超时";
                case DBResult.F_FILEMISSING: return "无法找到必要文件";
                case DBResult.F_DATACORRUPTED: return "数据失效";
                case DBResult.F_CONNECTION: return "无法连接服务器";
                default:
                    return "未知错误";
            }
        }

        public static bool Succeeded(DBResult res)
        {
            return res == DBResult.S_TRUE || res == DBResult.S_FALSE;
        }
    }
}
