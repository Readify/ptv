using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ptv.Timetable
{
    public class TimetableClient
    {
        private const string BaseUrl = "http://timetableapi.ptv.vic.gov.au";
        private const string HealthCheckPathAndQueryFormat = "/v2/healthcheck?timestamp={0}&";
        private const string SearchPathAndQueryFormat = "/v2/search/{0}?";
        private const string PathAndQueryWithDeveloperIDFormat = "{0}devid={1}";
        private const string PathAndQueryWithSignatureFormat = "{0}&signature={1}";
        public TimetableClient(string developerID, string securityKey, HmacSha1Hasher hasher)
        {
            this.DeveloperID = developerID;
            this.SecurityKey = securityKey;
            this.Hasher = hasher;
        }

        public string DeveloperID { get; private set; }
        public string SecurityKey { get; private set; }
        public HmacSha1Hasher Hasher { get; set; }

        private string ApplySignature(string pathAndQuery)
        {
            var pathAndQueryBytes = Encoding.UTF8.GetBytes(pathAndQuery);
            var securityKeyBytes = Encoding.UTF8.GetBytes(this.SecurityKey);
            var signatureBytes = this.Hasher(pathAndQueryBytes, securityKeyBytes);

            var builder = new StringBuilder();

            foreach (var signatureByte in signatureBytes)
            {
                builder.AppendFormat("{0:X2}", signatureByte);
            }

            var signature = builder.ToString();
            var signedPathAndQuery = string.Format(TimetableClient.PathAndQueryWithSignatureFormat, pathAndQuery, signature);
            return signedPathAndQuery;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(TimetableClient.BaseUrl);
            return client;
        }

        public async Task<SearchResult[]> SearchAsync(string keyword)
        {
            using (var client = this.GetHttpClient())
            {
                var searchPathAndQuery = string.Format(
                    TimetableClient.SearchPathAndQueryFormat,
                    keyword
                    );

                var searchPathAndQueryWithDeveloperID = string.Format(
                    TimetableClient.PathAndQueryWithDeveloperIDFormat,
                    searchPathAndQuery,
                    this.DeveloperID
                    );

                var signedSearchPathAndQuery = this.ApplySignature(searchPathAndQueryWithDeveloperID);

                var resultStream = await client.GetStreamAsync(signedSearchPathAndQuery);
                var resultStreamReader = new StreamReader(resultStream);
                var resultJsonReader = new JsonTextReader(resultStreamReader);

                var serializer = new JsonSerializer();
                var searchResult = serializer.Deserialize<SearchResult[]>(resultJsonReader);

                return searchResult;
            }
        }

        public async Task<Health> GetHealthAsync()
        {
            using (var client = this.GetHttpClient())
            {
                var healthCheckPathAndQuery = string.Format(
                    TimetableClient.HealthCheckPathAndQueryFormat,
                    DateTime.UtcNow.ToString("o")
                    );

                var healthCheckPathAndQueryWithDeveloperID = string.Format(
                    TimetableClient.PathAndQueryWithDeveloperIDFormat,
                    healthCheckPathAndQuery,
                    this.DeveloperID
                    );

                var signedHealthCheckPathAndQuery = this.ApplySignature(healthCheckPathAndQueryWithDeveloperID);

                var resultStream = await client.GetStreamAsync(signedHealthCheckPathAndQuery);
                var resultStreamReader = new StreamReader(resultStream);
                var resultJsonReader = new JsonTextReader(resultStreamReader);

                var serializer = new JsonSerializer();
                var health = serializer.Deserialize<Health>(resultJsonReader);
                return health;
            }
        }
    }
}
