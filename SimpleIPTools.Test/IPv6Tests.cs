using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleIPTools;

namespace SimpleIPTools.Test
{
    [TestClass]
    public class IPv6Tests
    {
        [TestMethod]
        public void ToUncompressedString_WithParsedIP_ReturnsCorrectUncompressedIP()
        {
            var ipAddress = System.Net.IPAddress.Parse("2001:db8:0:42:0:8a2e:370:7334");
            string expected = "2001:0DB8:0000:0042:0000:8A2E:0370:7334";
            Assert.AreEqual(expected, IPv6.ToUncompressedString(ipAddress));
        }

        [TestMethod]
        public void ToUncompressedString_WithStringIP_ReturnsCorrectUncompressedIP()
        {
            var ipAddress = "2001:db8:0:42:0:8a2e:370:7334";
            string expected = "2001:0DB8:0000:0042:0000:8A2E:0370:7334";
            Assert.AreEqual(expected, IPv6.ToUncompressedString(ipAddress));
        }

    }
}