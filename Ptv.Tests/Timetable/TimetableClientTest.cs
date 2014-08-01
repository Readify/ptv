using System;
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
        public async Task EnsureGetHealthCallReturnsHealthObjectWithIsOKSetToTrue()
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
        public async Task SearchSandbox()
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
            var result = await timetableClient.SearchAsync("Werribee");
        }
    }
}
