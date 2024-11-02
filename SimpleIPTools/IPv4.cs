using System.Net.Sockets;
using System.Net;
using LukeSkywalker.IPNetwork;

namespace SimpleIPTools
{
    public static class IPv4
    {
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

        private static long iMask(int s)
        {
            return (long)(Math.Pow(2, 32) - Math.Pow(2, (32 - s)));
        }
        private static string long2ip(long ipAddress)
        {
            if (System.Net.IPAddress.TryParse(ipAddress.ToString(), out var ip))
            {
                return ip.ToString();
            }
            return "";
        }
        private static long ip2long(string ipAddress)
        {
            if (System.Net.IPAddress.TryParse(ipAddress, out var ip))
            {
                return (((long)ip.GetAddressBytes()[0] << 24) | ((long)ip.GetAddressBytes()[1] << 16) | ((long)ip.GetAddressBytes()[2] << 8) | ip.GetAddressBytes()[3]);
            }
            return -1;
        }
        /// <summary>
        /// Remove an ip/subnet from a bigger ip/subnet
        /// </summary>
        /// <param name="address">the network address</param>
        /// <param name="remove">the network address to be removed</param>
        /// <returns>List of ip/subnet</returns>
        public static List<string> RemoveIP(IPNetwork address, IPNetwork remove)
        {
            var checkrange = new IPAddressRange(address.Network, address.Broadcast);
            if (checkrange.IsInRange(remove.Network) && checkrange.IsInRange(remove.Broadcast))
            {
                var list1 = Convert2CIDR(address.Network.ToString(), GetPreviousIP(remove.Network.ToString()));
                var list2 = Convert2CIDR(GetNextIP(remove.Broadcast.ToString()), address.Broadcast.ToString());
                list2.AddRange(list1);
                list2 = list2.Distinct().ToList();
                return list2;
            }
            else if (checkrange.IsInRange(remove.Network))
            {
                return Convert2CIDR(address.Network.ToString(), GetPreviousIP(remove.Network.ToString()));
            }
            else if (checkrange.IsInRange(remove.Broadcast))
            {
                return Convert2CIDR(GetNextIP(remove.Broadcast.ToString()), address.Broadcast.ToString());
            }
            return new List<string>() { address.ToString() };
        }
        public static string GetPreviousIP(string ipAddress)
        {
            return long2ip(ip2long(ipAddress) - 1);
        }
        public static string GetNextIP(string ipAddress)
        {
            return long2ip(ip2long(ipAddress) + 1);
        }

        public static List<string> Convert2CIDR(string ipStart, string ipEnd)
        {
            long start = ip2long(ipStart);
            long end = ip2long(ipEnd);
            var result = new List<string>();

            while (end >= start)
            {
                byte maxSize = 32;
                while (maxSize > 0)
                {
                    long mask = iMask(maxSize - 1);
                    long maskBase = start & mask;

                    if (maskBase != start)
                    {
                        break;
                    }

                    maxSize--;
                }
                double x = Math.Log(end - start + 1) / Math.Log(2);
                byte maxDiff = (byte)(32 - Math.Floor(x));
                if (maxSize < maxDiff)
                {
                    maxSize = maxDiff;
                }
                string ip = long2ip(start);
                result.Add(ip + "/" + maxSize);
                start += (long)Math.Pow(2, (32 - maxSize));
            }
            return result;
        }

        public static string ToUncompressedString(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)  // IPv4
            {
                var strings = ipAddress.GetAddressBytes()
                    .Select(b => string.Format("{0:D3}", b));   // format bytes with padded 0s

                return string.Join(".", strings);   // join padded strings with '.' character
            }

            return ipAddress.ToString();    // all else treat as to string
        }

        public static string ToUncompressedString(string ipAddress)
        {
            var _ipAddress = System.Net.IPAddress.Parse(ipAddress);
            if (_ipAddress.AddressFamily == AddressFamily.InterNetwork)  // IPv4
            {
                var strings = _ipAddress.GetAddressBytes()
                    .Select(b => string.Format("{0:D3}", b));   // format bytes with padded 0s

                return string.Join(".", strings);   // join padded strings with '.' character
            }

            return ipAddress.ToString();    // all else treat as to string
        }


    }
}