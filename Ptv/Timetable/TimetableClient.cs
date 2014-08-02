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
        private const string GetHealthPathAndQueryFormat = "/v2/healthcheck?timestamp={0}&";
        private const string GetNearbyPathAndQueryFormat = "/v2/nearme/latitude/{0}/longitude/{1}?";
        private const string GetPointsOfInterestPathAndQueryFormat = "/v2/poi/{0}/lat1/{1}/long1/{2}/lat2/{3}/long2/{4}/griddepth/{5}/limit/{6}?";
        private const string SearchPathAndQueryFormat = "/v2/search/{0}?";
        private const string GetBroadNextDeparturesPathAndQueryFormat = "/v2/mode/{0}/stop/{1}/departures/by-destination/limit/{2}?";
        private const string GetSpecificNextDeparturesPathAndQueryFormat = "/v2/mode/{0}/line/{1}/stop/{2}/directionid/{3}/departures/all/limit/{4}?for_utc={5}&";
        private const string GetStoppingPatternPathAndQueryFormat = "/v2/mode/{0}/run/{1}/stop/{2}/stopping-pattern?for_utc={3}&";
        private const string DeveloperIDFormat = "{0}devid={1}";
        private const string SignatureFormat = "{0}&signature={1}";
        
        public TimetableClient(string developerID, string securityKey, TimetableClientHasher hasher)
        {
            this.DeveloperID = developerID;
            this.SecurityKey = securityKey;
            this.Hasher = hasher;
        }

        public string DeveloperID { get; private set; }
        public string SecurityKey { get; private set; }
        public TimetableClientHasher Hasher { get; private set; }

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
                var result = JsonConvert.DeserializeObject<T>(
                    json,
                    new DepartureConverter(),
                    new ItemConverter(),
                    new LocationConverter()
                    );

                return result;
            }
        }

        public async Task<Health> GetHealthAsync()
        {
            var timestampInIso8601 = DateTime.UtcNow.ToString("o");
            var pathAndQuery = string.Format(TimetableClient.GetHealthPathAndQueryFormat, timestampInIso8601);
            var result = await this.ExecuteAsync<Health>(pathAndQuery);
            return result;
        }

        public async Task<Stop[]> GetNearbyStops(decimal latitude, decimal longitude)
        {
            var pathAndQuery = string.Format(TimetableClient.GetNearbyPathAndQueryFormat, latitude, longitude);
            var result = await this.ExecuteAsync<Item[]>(pathAndQuery);
            var castResult = result.Cast<Stop>().ToArray();
            return castResult;
        }

        public async Task<Item[]> SearchAsync(string keyword)
        {
            var encodedKeyword = Uri.EscapeDataString(keyword);
            var pathAndQuery = string.Format(TimetableClient.SearchPathAndQueryFormat, encodedKeyword);
            var result = await this.ExecuteAsync<Item[]>(pathAndQuery);
            return result;
        }

        public async Task<PointsOfInterest> GetPointsOfInterest(PointOfInterestType pointOfInterestType, decimal topLeftLatitude, decimal topLeftLongitude, decimal bottomRightLatitude, decimal bottomRightLongitude, uint gridDepth, uint limit)
        {
            var pathAndQuery = string.Format(
                TimetableClient.GetPointsOfInterestPathAndQueryFormat,
                (uint)pointOfInterestType,
                topLeftLatitude,
                topLeftLongitude,
                bottomRightLatitude,
                bottomRightLongitude,
                gridDepth,
                limit
                );
            var result = await this.ExecuteAsync<PointsOfInterest>(pathAndQuery);
            return result;
        }

        public async Task<Departure[]> GetBroadNextDepartures(TransportType mode, uint stopID, uint limit)
        {
            var pathAndQuery = string.Format(
                TimetableClient.GetBroadNextDeparturesPathAndQueryFormat,
                (uint)mode,
                stopID,
                limit
                );
            var result = await this.ExecuteAsync<Departure[]>(pathAndQuery);
            return result;

        }

        public async Task<Departure[]> GetSpecificNextDepartures(TransportType mode, string lineID, string stopID, string directionID, uint limit, DateTime fromUtc)
        {
            var pathAndQuery = string.Format(
                TimetableClient.GetSpecificNextDeparturesPathAndQueryFormat,
                (uint)mode,
                lineID,
                stopID,
                directionID,
                limit,
                fromUtc.ToString("o")
                );
            var result = await this.ExecuteAsync<Departure[]>(pathAndQuery);
            return result;
        }

        public async Task<Departure[]> GetStoppingPattern(TransportType mode, string runID, string stopID, DateTime fromUtc)
        {
            var pathAndQuery = string.Format(
                TimetableClient.GetStoppingPatternPathAndQueryFormat,
                (uint)mode,
                runID,
                stopID,
                fromUtc.ToString("o")
                );
            var result = await this.ExecuteAsync<Departure[]>(pathAndQuery);
            return result;
        }
    }
}
