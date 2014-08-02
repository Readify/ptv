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


        [TestMethod]
        public async Task EnsureHealthCallReturnsHealthObjectWithIsOKSetToTrue()
        {
            // Arrange.
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

            // Act.
            var health = await timetableClient.GetHealthAsync();

            // Assert.
            Assert.IsTrue(health.IsOK);
        }

        [TestMethod]
        public async Task EnsureSearchAsyncCallReturnsMoreThanOneTypeOfItem()
        {
            // Arrange.
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

            // Act.
            var results = await timetableClient.SearchAsync("Werribee");
            var resultGroupedByType = results.GroupBy((item) => item.GetType());
            var countOfGroupedResults = resultGroupedByType.Count();

            // Assert.
            Assert.IsTrue(countOfGroupedResults > 1);
        }
    }
}
