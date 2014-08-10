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
            var results = await timetableClient.GetNearbyStopsAsync(-37.8136, 144.9631);
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
            var results = await timetableClient.GetPointsOfInterestAsync(
                PointOfInterestType.Train,
                -37.9000, 144.6640, -37.8136, 144.9631,
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
            var results = await timetableClient.GetBroadNextDeparturesAsync(TransportType.Bus, "19112", 1);
        }

        [TestMethod()]
        public async Task EnsureGetSpecificNextDeparturesReturnsResults()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.GetSpecificNextDeparturesAsync(TransportType.Bus, "979", "19112", "204", 1, DateTime.UtcNow);
        }

        [TestMethod()]
        public async Task EnsureGetStoppingPatternReturnsResults()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.GetStoppingPatternAsync(TransportType.Bus, "25808", "19112", DateTime.UtcNow);
        }

        [TestMethod()]
        public async Task EnsureGetLineStopsReturnsResults()
        {
            // Arrange.
            var timetableClient = this.GetTimetableClient();

            // Act.
            var results = await timetableClient.GetLineStopsAsync(TransportType.Bus, "987");
        }
    }
}
