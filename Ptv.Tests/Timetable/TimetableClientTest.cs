using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ptv.Timetable;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Ptv.Tests.Timetable
{
    [TestClass]
    public class TimetableClientTest
    {
        private TimetableClient GetTimetableClient()
        {
            var hasher = new SHA1CryptoServiceProvider();
            var timetableClient = new TimetableClient(
                TestConstants.TimetableDeveloperID,
                TestConstants.TimetableSecurityKey,
                (input, key) =>
                {
                    var provider = new HMACSHA1(key);
                    var hash = provider.ComputeHash(input);
                    return hash;
                });
            return timetableClient;
        }

        [TestMethod()]
        public async Task EnsureGetHealthAsyncReturnsHealthObjectWithIsOKSetToTrue()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var health = await timetableClient.GetHealthAsync();

            // Assert.
            Assert.IsTrue(health.IsOK);
        }

        [TestMethod()]
        public async Task EnsureSearchAsyncReturnsMoreThanOneTypeOfItem()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.SearchAsync("Werribee");
            var resultsGroupedByType = results.GroupBy((item) => item.GetType());
            var countOfGroupedResults = resultsGroupedByType.Count();

            // Assert.
            Assert.IsTrue(countOfGroupedResults > 1);
        }

        [TestMethod()]
        public async Task EnsureGetNearbyAsyncReturnsItems()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.GetNearbyStops(-37.8136m, 144.9631m);
            var resultsGroupedByTransportType = results.GroupBy((stop) => stop.TransportType);
            var countOfGroupedResults = resultsGroupedByTransportType.Count();

            // Assert.
            Assert.IsTrue(countOfGroupedResults == 3);
        }
        
        [TestMethod()]
        public async Task EnsureGetPointsOfInterestReturnsResults()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.GetPointsOfInterest(
                PointOfInterestType.Train,
                -37.9000m, 144.6640m, -37.8136m, 144.9631m,
                2,
                10
                );
        }

        [TestMethod()]
        public async Task EnsureGetBroadNextDeparturesReturnsResults()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();
            
            // Act.
            var results = await timetableClient.GetBroadNextDepartures(TransportType.Bus, 19112, 1);
        }
    }
}
