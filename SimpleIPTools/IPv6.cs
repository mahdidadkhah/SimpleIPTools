using System.Net.Sockets;
using System.Net;

namespace SimpleIPTools
{
    public static class IPv6
    {
        public static string ToUncompressedString(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)    // IPv6
            {

                var strings = Enumerable.Range(0, 8)    // create index
                                        .Select(i => ipAddress.GetAddressBytes().ToList().GetRange(i * 2, 2))       // get 8 chunks of bytes
                                        .Select(i => { i.Reverse(); return i; })    // reverse bytes for endianness
                                        .Select(bytes => BitConverter.ToInt16(bytes.ToArray(), 0))  // convert bytes to 16 bit int
                                        .Select(int16 => string.Format("{0:X4}", int16).ToUpper()); // format int as a 4 digit hex 

                return string.Join(":", strings);   // join hex ints with ':'
            }

            return ipAddress.ToString();    // all else treat as to string
        }

        public class IPAddressRange
        {
            readonly AddressFamily addressFamily;
            readonly byte[] lowerBytes;
            readonly byte[] upperBytes;

            public IPAddressRange(IPAddress lowerInclusive, IPAddress upperInclusive)
            {
                this.addressFamily = lowerInclusive.AddressFamily;
                this.lowerBytes = lowerInclusive.GetAddressBytes();
                this.upperBytes = upperInclusive.GetAddressBytes();
            }

            public bool IsInRange(IPAddress address)
            {
                if (address.AddressFamily != addressFamily)
                {
                    return false;
                }

                byte[] addressBytes = address.GetAddressBytes();

                bool lowerBoundary = true, upperBoundary = true;

                for (int i = 0; i < this.lowerBytes.Length &&
                    (lowerBoundary || upperBoundary); i++)
                {
                    if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
                        (upperBoundary && addressBytes[i] > upperBytes[i]))
                    {
                        return false;
                    }
                    lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                    upperBoundary &= (addressBytes[i] == upperBytes[i]);
                }
                return true;
            }
        }
    }
}