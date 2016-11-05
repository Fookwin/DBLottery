using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace DBSQLService.Data
{
    public class DBSQLClient
    {
        private static DBSQLClient _instance = null;

        private DBHistoryEntity context = new DBHistoryEntity();

        public static DBSQLClient Instance()
        {
            if (_instance == null)
                _instance = new DBSQLClient();

            return _instance;
        }

        private DBSQLClient()
        {
        }

        public int GetRecordCount()
        {
            return context.Basics.Count();
        }

        public void GetRecordList(out List<Basic> basicList, out List<Detail> detailList, 
            out List<Omission> omissionList, out List<Attribute> attributeList)
        {
            basicList = context.Basics.ToList();
            detailList = context.Details.ToList();
            omissionList = context.Omissions.ToList();
            attributeList = context.Attributes.ToList();
        } 

        public bool GetRecord(int issue, out Basic basic, out Detail detail,
            out Omission omission, out Attribute attribute)
        {
            basic = context.Basics.Find(issue);
            detail = context.Details.Find(issue);
            omission = context.Omissions.Find(issue);
            attribute = context.Attributes.Find(issue);
            if (basic == null || detail == null || omission == null || attribute == null)
                return false;

            return true;
        }

        public bool GetRecordBasic(int issue, out Basic basic, out Detail detail)
        {
            basic = context.Basics.Find(issue);
            detail = context.Details.Find(issue);
            if (basic == null || detail == null)
                return false;

            return true;
        }

        public bool GetLastRecord(out Basic basic, out Detail detail, out Omission omission, out Attribute attribute)
        {
            basic = context.Basics.AsEnumerable().Last();
            detail = context.Details.AsEnumerable().Last();
            omission = context.Omissions.AsEnumerable().Last();
            attribute = context.Attributes.AsEnumerable().Last();
            if (basic == null || detail == null || omission == null || attribute == null)
                return false;

            return true;
        }

        public int GetLastIssue()
        {
            Basic basic = context.Basics.AsEnumerable().Last();
            return basic.Issue;
        }

        public DateTime GetLastReleaseTime()
        {
            Detail detail = context.Details.AsEnumerable().Last();
            return detail.Date;
        }

        public string ExecuteSqlQueries(string sqlLines)
        {
            try
            {
                System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder();
                builder["Server"] = "ppuvjzarol.database.windows.net";
                builder["User ID"] = "pi3011314@ppuvjzarol.database.windows.net";
                builder["Password"] = "zzx&jjj1314";

                builder["Database"] = "dbhistory";
                builder["Trusted_Connection"] = false;
                builder["Integrated Security"] = false;
                builder["Encrypt"] = true;

                //1. Define an Exponential Backoff retry strategy for Azure SQL Database throttling (ExponentialBackoff Class). An exponential back-off strategy will gracefully back off the load on the service.
                int retryCount = 4;
                int minBackoffDelayMilliseconds = 2000;
                int maxBackoffDelayMilliseconds = 8000;
                int deltaBackoffMilliseconds = 2000;

                ExponentialBackoff exponentialBackoffStrategy =
                  new ExponentialBackoff("exponentialBackoffStrategy",
                      retryCount,
                      TimeSpan.FromMilliseconds(minBackoffDelayMilliseconds),
                      TimeSpan.FromMilliseconds(maxBackoffDelayMilliseconds),
                      TimeSpan.FromMilliseconds(deltaBackoffMilliseconds));

                //2. Set a default strategy to Exponential Backoff.
                RetryManager manager = new RetryManager(new List<RetryStrategy>
                {
                    exponentialBackoffStrategy
                }, "exponentialBackoffStrategy");

                //3. Set a default Retry Manager. A RetryManager provides retry functionality, or if you are using declarative configuration, you can invoke the RetryPolicyFactory.CreateDefault
                RetryManager.SetDefault(manager);

                //4. Define a default SQL Connection retry policy and SQL Command retry policy. A policy provides a retry mechanism for unreliable actions and transient conditions.
                RetryPolicy retryConnectionPolicy = manager.GetDefaultSqlConnectionRetryPolicy();
                RetryPolicy retryCommandPolicy = manager.GetDefaultSqlCommandRetryPolicy();

                //5. Create a function that will retry the connection using a ReliableSqlConnection.
                retryConnectionPolicy.ExecuteAction(() =>
                {
                    using (ReliableSqlConnection connection = new ReliableSqlConnection(builder.ConnectionString))
                    {
                        connection.Open();

                        IDbCommand command = connection.CreateCommand();
                        command.CommandText = sqlLines;

                        //6. Create a function that will retry the command calling ExecuteCommand() from the ReliableSqlConnection
                        retryCommandPolicy.ExecuteAction(() =>
                        {
                            using (IDataReader reader = connection.ExecuteCommand<IDataReader>(command))
                            {
                                while (reader.Read())
                                {
                                    string name = reader.GetString(0);
                                    Console.WriteLine(name);
                                }
                            }
                        });
                    }
                });
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Success";
        }
    }
}