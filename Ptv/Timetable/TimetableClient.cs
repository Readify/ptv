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
        private const string StopsNearbyPathAndQueryFormat = "/v2/nearme/latitude/{0}/longitude/{1}?";
        private const string SearchPathAndQueryFormat = "/v2/search/{0}?";
        private const string DeveloperIDFormat = "{0}devid={1}";
        private const string SignatureFormat = "{0}&signature={1}";
        
        public TimetableClient(string developerID, string securityKey, HmacSha1Hasher hasher)
        {
            this.DeveloperID = developerID;
            this.SecurityKey = securityKey;
            this.Hasher = hasher;
        }

        public string DeveloperID { get; private set; }
        public string SecurityKey { get; private set; }
        public HmacSha1Hasher Hasher { get; private set; }

        private string ApplySignature(string pathAndQuery)
        {
            var pathAndQueryBytes = Encoding.UTF8.GetBytes(pathAndQuery);
            var securityKeyBytes = Encoding.UTF8.GetBytes(this.SecurityKey);

            var signatureBytes = this.Hasher(pathAndQueryBytes, securityKeyBytes);
            var signature = this.EncodeSignature(signatureBytes);

            var pathAndQueryWithSignature = string.Format(TimetableClient.SignatureFormat, pathAndQuery, signature);
            return pathAndQueryWithSignature;
        }

        private string EncodeSignature(byte[] signatureBytes)
        {
            var builder = new StringBuilder();
            foreach (var signatureByte in signatureBytes)
            {
                builder.AppendFormat("{0:X2}", signatureByte);
            }

            var signature = builder.ToString();
            return signature;
        }

        private string ApplyDeveloperID(string pathAndQuery)
        {
            var pathAndQueryWithDeveloperID = string.Format(
              TimetableClient.DeveloperIDFormat,
              pathAndQuery,
              this.DeveloperID
              );

            return pathAndQueryWithDeveloperID;
        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(TimetableClient.BaseUrl);
            return client;
        }

        private async Task<T> ExecuteAsync<T>(string pathAndQuery)
        {
            var pathAndQueryWithDeveloperID = this.ApplyDeveloperID(pathAndQuery);
            var pathAndQueryWithDeveloperIDAndSignature = this.ApplySignature(pathAndQueryWithDeveloperID);

            using (var client = this.GetHttpClient())
            {
                var json = await client.GetStringAsync(pathAndQueryWithDeveloperIDAndSignature);
                var result = JsonConvert.DeserializeObject<T>(json, new ItemConverter());
                return result;
            }
        }

        public async Task<Health> GetHealthAsync()
        {
            var timestampInIso8601 = DateTime.UtcNow.ToString("o");
            var pathAndQuery = string.Format(TimetableClient.HealthCheckPathAndQueryFormat, timestampInIso8601);
            var result = await this.ExecuteAsync<Health>(pathAndQuery);
            return result;
        }

        public async Task<Item[]> SearchNearbyAsync(decimal latitude, decimal longitude)
        {
            var pathAndQuery = string.Format(TimetableClient.StopsNearbyPathAndQueryFormat, latitude, longitude);
            var result = await this.ExecuteAsync<Item[]>(pathAndQuery);
            return result;
        }

        public async Task<Item[]> SearchAsync(string keyword)
        {
            var pathAndQuery = string.Format(TimetableClient.SearchPathAndQueryFormat, keyword);
            var result = await this.ExecuteAsync<Item[]>(pathAndQuery);
            return result;

        }

    }
}
