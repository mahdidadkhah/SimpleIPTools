using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleIPTools;

namespace SimpleIPTools.Test
{
    [TestClass]
    public class IPv4Tests
    {
        [TestMethod]
        public void ToUncompressedString_WithParsedIP_ReturnsCorrectUncompressedIP()
        {
            var ipAddress = System.Net.IPAddress.Parse("127.0.0.1");
            string expected = "127.000.000.001";
            Assert.AreEqual(expected, IPv4.ToUncompressedString(ipAddress));
        }

        public void ToUncompressedString_WithStringIP_ReturnsCorrectUncompressedIP()
        {
            var ipAddress = "127.0.0.1";
            string expected = "127.000.000.001";
            Assert.AreEqual(expected, IPv4.ToUncompressedString(ipAddress));
        }

        [TestMethod]
        public void GetNextIp_ValidIP_ReturnsNextIP()
        {
            var ipAddress = "192.168.1.1";
            string expected = "192.168.1.2";
            Assert.AreEqual(expected, IPv4.GetNextIP(ipAddress));
        }

        [TestMethod]
        public void GetPrevoGetPreviousIP_ValiIP_ReturnsPreviousIP()
        {
            var ipAddress = "192.168.1.2";
            string expected = "192.168.1.1";
            Assert.AreEqual(expected, IPv4.GetPreviousIP(ipAddress));
        }

        [TestMethod]
        public void Convert2CIDR_OneValidRange_ReturnsCorrectCIDR()
        {
            // Arrange
            string ipStart = "192.168.0.0";
            string ipEnd = "192.168.0.255";
            var expectedResult = new List<string> { "192.168.0.0/24" };

            // Act
            var result = IPv4.Convert2CIDR(ipStart, ipEnd);

            // Assert
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Convert2CIDR_SingleIP_ReturnsCorrectCIDR()
        {
            // Arrange
            string ipStart = "192.168.0.1";
            string ipEnd = "192.168.0.1";
            var expectedResult = new List<string> { "192.168.0.1/32" };

            // Act
            var result = IPv4.Convert2CIDR(ipStart, ipEnd);

            // Assert
            CollectionAssert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void Convert2CIDR_MoreThanOneValidRange_ReturnsCorrectCIDR()
        {
            // Arrange
            string ipStart = "192.168.1.0";
            string ipEnd = "192.168.1.59";
            var expectedResult = new List<string> { "192.168.1.0/27", "192.168.1.32/28", "192.168.1.48/29", "192.168.1.56/30" };

            // Act
            var result = IPv4.Convert2CIDR(ipStart, ipEnd);

            // Assert
            CollectionAssert.AreEquivalent(expectedResult, result);
        }

    }
}