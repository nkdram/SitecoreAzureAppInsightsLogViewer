using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using RD.SALV.CORE.Models;
using System.Linq;

namespace RD.SALV.CORE.Services
{
    public class AzureLogViewerService : ILogViewerService
    {
        /// <summary>
        /// App Insights Key
        /// </summary>
        public static string _appInsightsApiKey { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public static string _appInsightsID { get; set; }


        /// <summary>
        /// App Insights Api EP
        /// </summary>
        private const string _url = "https://api.applicationinsights.io/v1/apps/{0}/{1}?{2}";
        /// <summary>
        /// Default Query type is Trace as we want only Sitecore logs
        /// </summary>
        private const string _defaultQueryType = "query";

        // private const string _defaultQueryPath = "traces";

        public AzureLogViewerService(string appId, string apiKey)
        {
            _appInsightsID = appId;
            _appInsightsApiKey = apiKey;
        }


        public LogResponseData GetLogs(LogRequestData LogreqData)
        {
            string responseData = GetTelemetry(BuildQuery(LogreqData));
            Models.TableData azureTables = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.TableData>(responseData);

            /// if table data returned is empty
            if (azureTables.tables.Count == 0)
                return new LogResponseData()
                {
                    ContainsErrors = false,
                    NumberOfRows = 0,
                    Logs = new System.Collections.Generic.List<LogData>()
                };

            return new LogResponseData()
            {
                ContainsErrors = false,
                NumberOfRows = azureTables.tables[0].rows.Count, // take default table
                Logs = azureTables.tables[0].rows.Select(x => new LogData() { TimeStamp = DateTime.Parse(x[0]), LogMessage = x[1], LogType = this.ParseEnum<LogType>(x[2]) }).ToList()
            };
        }

        /// <summary>
        /// Builds and returns query based on Log Request Data
        /// </summary>
        /// <param name="LogreqData"></param>
        /// <returns></returns>
        private string BuildQuery(LogRequestData LogreqData)
        {
            //string queryDetails = $"traces | where cloud_RoleInstance == 'RD281878654B81' | where timestamp >= datetime(2019 - 10 - 01T09: 30:00Z) and timestamp <= datetime(2019 - 10 - 06T10: 30:00Z) | where severityLevel == 3 | project timestamp, message,severityLevel,cloud_RoleName | order by timestamp desc"
            string msgQuery = string.Empty;
            if (!string.IsNullOrEmpty(LogreqData.Message) && !string.IsNullOrEmpty(LogreqData.ExpressionType))
            {
                msgQuery = $" | where message {ToDescriptionString(LogreqData.ExpressionType)} '{LogreqData.Message}'";
            }
            var logTypes = LogreqData.LogType.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string severityLevel = "";
            foreach (var logtype in logTypes)
            {
                if (string.IsNullOrEmpty(severityLevel))
                    severityLevel += $" severityLevel == {logtype}";
                else
                    severityLevel += $" or severityLevel == {logtype}";
            }

            string limit = $" | limit { (LogreqData.Limit > 5000 ? "5000" : LogreqData.Limit.ToString())}";

            return System.Web.HttpUtility.UrlPathEncode($"query= traces | where cloud_RoleInstance == '{LogreqData.CloudInstance}' {msgQuery} | where timestamp >= datetime('{LogreqData.FromDate.ToUniversalTime().ToString()}') and timestamp <= datetime('{LogreqData.ToDate.ToUniversalTime().ToString()}') | where {severityLevel} | project timestamp, message,severityLevel,cloud_RoleName { limit } | order by timestamp desc");
        }

        /// <summary>
        /// Converts Message Expression
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string ToDescriptionString(string val)
        {
            Enum.TryParse(val, out MessageExpresssion messageExp);
            DescriptionAttribute[] attributes = (DescriptionAttribute[])messageExp
               .GetType()
               .GetField(messageExp.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }

        /// <summary>
        /// Returns Telemetry data
        /// </summary>
        /// <param name="parameterString"></param>
        /// <returns></returns>
        private string GetTelemetry(string parameterString)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", _appInsightsApiKey);
            var req = string.Format(_url, _appInsightsID, _defaultQueryType, (parameterString));
            HttpResponseMessage response = client.GetAsync(req).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return response.ReasonPhrase;
            }
        }

        /// <summary>
        /// returns cloud instances
        /// </summary>
        /// <returns></returns>
        public CloudInstanceResponseData GetCloudInstances()
        {
            string cloudInstanceQuery = System.Web.HttpUtility.UrlPathEncode(@"query=traces | extend scinstancename=parsejson(customDimensions).InstanceName
                            | where timestamp > now(-1d)
                            | summarize count(), any(tostring(scinstancename)) by cloud_RoleInstance
                            | extend InstanceName=any_scinstancename
                            | extend CloudRole=cloud_RoleInstance
                            | project InstanceName, CloudRole
                            | order by InstanceName asc");

            var cloudInstanceData = GetTelemetry(cloudInstanceQuery);
            Models.TableData azureTables = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.TableData>(cloudInstanceData);

            /// if table data returned is empty
            if (azureTables.tables.Count == 0)
                return new CloudInstanceResponseData()
                {
                    ContainsErrors = false,
                    NumberOfRows = 0,
                    Logs = new System.Collections.Generic.List<CloudInstancesData>()
                };

            return new CloudInstanceResponseData()
            {
                ContainsErrors = false,
                NumberOfRows = azureTables.tables[0].rows.Count, // take default table
                Logs = azureTables.tables[0].rows.Select(x => new CloudInstancesData() { InstanceName = x[0], CloudRole = x[1] }).ToList()
            };
        }

        /// <summary>
        /// ParseEnum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}