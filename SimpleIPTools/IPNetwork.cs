using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace LukeSkywalker.IPNetwork {
    /// <summary>
    /// IP Network utility class. 
    /// Use IPNetwork.Parse to create instances.
    /// </summary>
    public class IPNetwork : IComparable<IPNetwork> {

        #region properties

        //private uint _network;
        private uint _ipaddress;
        //private uint _netmask;
        //private uint _broadcast;
        //private uint _firstUsable;
        //private uint _lastUsable;
        //private uint _usable;
        private byte _cidr;

        #endregion

        #region accessors

        private uint _network {
            get {
                uint uintNetwork = this._ipaddress & this._netmask;
                return uintNetwork;
            }
        }

        /// <summary>
        /// Network address
        /// </summary>
        public IPAddress Network {
            get {

                return IPNetwork.ToIPAddress(this._network);
            }
        }

        private uint _netmask
        {
            get
            {
                return IPNetwork.ToUint(this._cidr);
            }
        }
        /// <summary>
        /// Netmask
        /// </summary>
        public IPAddress Netmask
        {
            get {
                return IPNetwork.ToIPAddress(this._netmask);
            }
        }

        private uint _broadcast {
            get {
                 uint uintBroadcast = this._network + ~this._netmask;
                 return uintBroadcast;
            }
        }
        /// <summary>
        /// Broadcast address
        /// </summary>
        public IPAddress Broadcast {
            get {
                
                return IPNetwork.ToIPAddress(this._broadcast);
            }
        }

        /// <summary>
        /// First usable IP adress in Network
        /// </summary>
        public IPAddress FirstUsable {
            get {
                uint uintFirstUsable = (this.Usable <= 0) ? this._network : this._network + 1;
                return IPNetwork.ToIPAddress(uintFirstUsable);
            }
        }

        /// <summary>
        /// Last usable IP adress in Network
        /// </summary>
        public IPAddress LastUsable
        {
            get {
                uint uintLastUsable = (this.Usable <= 0) ? this._network : this._broadcast - 1;
                return IPNetwork.ToIPAddress(uintLastUsable);
            }
        }

        /// <summary>
        /// Number of usable IP adress in Network
        /// </summary>
        public uint Usable
        {
            get {
                int cidr = IPNetwork.ToCidr(this._netmask);
                uint usableIps = (cidr > 30) ? 0 : ((0xffffffff >> cidr) - 1);
                return usableIps;
            }
        }

        /// <summary>
        /// The CIDR netmask notation
        /// </summary>
        public byte Cidr {
            get {
                return this._cidr;
            }
        }

        #endregion

        #region constructor

        internal IPNetwork(uint ipaddress, byte cidr)  { 

            if (cidr > 32)
            {
                throw new ArgumentOutOfRangeException("cidr");
            }

            this._ipaddress = ipaddress;
            this._cidr = cidr;

        }

        #endregion

        #region parsers

        /// <summary>
        /// 192.168.168.100 - 255.255.255.0
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static IPNetwork Parse(string ipaddress, string netmask) {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, netmask, out ipnetwork);
            return ipnetwork;
        }

        /// <summary>
        /// 192.168.168.100/24
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static IPNetwork Parse(string ipaddress, byte cidr) {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, cidr, out ipnetwork);
            return ipnetwork;

        }

        /// <summary>
        /// 192.168.168.100 255.255.255.0
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static IPNetwork Parse(IPAddress ipaddress, IPAddress netmask) {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, ipaddress, netmask, out ipnetwork);
            return ipnetwork;

        }

        /// <summary>
        /// 192.168.0.1/24
        /// 192.168.0.1 255.255.255.0
        /// 
        /// Network   : 192.168.0.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.0.1
        /// End       : 192.168.0.254
        /// Broadcast : 192.168.0.255
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static IPNetwork Parse(string network) {

            IPNetwork ipnetwork = null;
            IPNetwork.InternalParse(false, network, out ipnetwork);
            return ipnetwork;

        }

        
        #endregion

        #region TryParse



        /// <summary>
        /// 192.168.168.100 - 255.255.255.0
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static bool TryParse(string ipaddress, string netmask, out IPNetwork ipnetwork) {

            IPNetwork ipnetwork2 = null;
            IPNetwork.InternalParse(true, ipaddress, netmask, out ipnetwork2);
            bool parsed = (ipnetwork2 != null);
            ipnetwork = ipnetwork2;
            return parsed;

        }



        /// <summary>
        /// 192.168.168.100/24
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool TryParse(string ipaddress, byte cidr, out IPNetwork ipnetwork) {

            IPNetwork ipnetwork2 = null;
            IPNetwork.InternalParse(true, ipaddress, cidr, out ipnetwork2);
            bool parsed = (ipnetwork2 != null);
            ipnetwork = ipnetwork2;
            return parsed;

        }

        /// <summary>
        /// 192.168.0.1/24
        /// 192.168.0.1 255.255.255.0
        /// 
        /// Network   : 192.168.0.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.0.1
        /// End       : 192.168.0.254
        /// Broadcast : 192.168.0.255
        /// </summary>
        /// <param name="network"></param>
        /// <param name="ipnetwork"></param>
        /// <returns></returns>
        public static bool TryParse(string network, out IPNetwork ipnetwork) {

            IPNetwork ipnetwork2 = null;
            IPNetwork.InternalParse(true, network, out ipnetwork2);
            bool parsed = (ipnetwork2 != null);
            ipnetwork = ipnetwork2;
            return parsed;

        }

        /// <summary>
        /// 192.168.0.1/24
        /// 192.168.0.1 255.255.255.0
        /// 
        /// Network   : 192.168.0.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.0.1
        /// End       : 192.168.0.254
        /// Broadcast : 192.168.0.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <param name="ipnetwork"></param>
        /// <returns></returns>
        public static bool TryParse(IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork) {

            IPNetwork ipnetwork2 = null;
            IPNetwork.InternalParse(true, ipaddress, netmask, out ipnetwork2);
            bool parsed = (ipnetwork2 != null);
            ipnetwork = ipnetwork2;
            return parsed;

        }

        
        #endregion

        #region InternalParse

        /// <summary>
        /// 192.168.168.100 - 255.255.255.0
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private static void InternalParse(bool tryParse, string ipaddress, string netmask, out IPNetwork ipnetwork) {

            if (string.IsNullOrEmpty(ipaddress)) {
                if (tryParse == false) {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            if (string.IsNullOrEmpty(netmask)) {
                if (tryParse == false) {
                    throw new ArgumentNullException("netmask");
                }
                ipnetwork = null;
                return;
            }

            IPAddress ip = null;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);
            if (ipaddressParsed == false) {
                if (tryParse == false) {
                    throw new ArgumentException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask = null;
            bool netmaskParsed = IPAddress.TryParse(netmask, out mask);
            if (netmaskParsed == false) {
                if (tryParse == false) {
                    throw new ArgumentException("netmask");
                }
                ipnetwork = null;
                return;
            }

            IPNetwork.InternalParse(tryParse, ip, mask, out ipnetwork);
        }

        private static void InternalParse(bool tryParse, string network, out IPNetwork ipnetwork) {

            if (string.IsNullOrEmpty(network)) {
                if (tryParse == false) {
                    throw new ArgumentNullException("network");
                }
                ipnetwork = null;
                return;
            }

            network = Regex.Replace(network, @"[^0-9\.\/\s]+", "");
            network = Regex.Replace(network, @"\s{2,}", " ");
            network = network.Trim();
            string[] args = network.Split(new char[] { ' ', '/' });
            byte cidr = 0;
            if (args.Length == 1) {

                if (IPNetwork.TryGuessCidr(args[0], out cidr)) {
                    IPNetwork.InternalParse(tryParse, args[0], cidr, out ipnetwork);
                    return;
                }

                if (tryParse == false) {
                    throw new ArgumentException("network");
                }
                ipnetwork = null;
                return;
            }

            if (byte.TryParse(args[1], out cidr)) {
                IPNetwork.InternalParse(tryParse, args[0], cidr, out ipnetwork);
                return;
            }

            IPNetwork.InternalParse(tryParse, args[0], args[1], out ipnetwork);
            return;

        }



        /// <summary>
        /// 192.168.168.100 255.255.255.0
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private static void InternalParse(bool tryParse, IPAddress ipaddress, IPAddress netmask, out IPNetwork ipnetwork) {

            if (ipaddress == null) {
                if (tryParse == false) {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            if (netmask == null) {
                if (tryParse == false) {
                    throw new ArgumentNullException("netmask");
                }
                ipnetwork = null;
                return;
            }

            uint uintIpAddress = IPNetwork.ToUint(ipaddress);
            byte? cidr2 = null;
            bool parsed = IPNetwork.TryToCidr(netmask, out cidr2);
            if (parsed == false) {
                if (tryParse == false) {
                    throw new ArgumentException("netmask");
                }
                ipnetwork = null;
                return;
            }
            byte cidr = (byte)cidr2;

            IPNetwork ipnet = new IPNetwork(uintIpAddress, cidr);
            ipnetwork = ipnet;

            return;
        }



        /// <summary>
        /// 192.168.168.100/24
        /// 
        /// Network   : 192.168.168.0
        /// Netmask   : 255.255.255.0
        /// Cidr      : 24
        /// Start     : 192.168.168.1
        /// End       : 192.168.168.254
        /// Broadcast : 192.168.168.255
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <param name="cidr"></param>
        /// <returns></returns>
        private static void InternalParse(bool tryParse, string ipaddress, byte cidr, out IPNetwork ipnetwork) {

            if (string.IsNullOrEmpty(ipaddress)) {
                if (tryParse == false) {
                    throw new ArgumentNullException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            
            IPAddress ip = null;
            bool ipaddressParsed = IPAddress.TryParse(ipaddress, out ip);
            if (ipaddressParsed == false) {
                if (tryParse == false) {
                    throw new ArgumentException("ipaddress");
                }
                ipnetwork = null;
                return;
            }

            IPAddress mask = null;
            bool parsedNetmask = IPNetwork.TryToNetmask(cidr, out mask);
            if (parsedNetmask == false) {
                if (tryParse == false) {
                    throw new ArgumentException("cidr");
                }
                ipnetwork = null;
                return;
            }


            IPNetwork.InternalParse(tryParse, ip, mask, out ipnetwork);
        }

        #endregion

        #region converters

        #region ToUint

        /// <summary>
        /// Convert an ipadress to decimal
        /// 0.0.0.0 -> 0
        /// 0.0.1.0 -> 256
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static uint ToUint(IPAddress ipaddress) {
            uint? uintIpAddress = null;
            IPNetwork.InternalToUint(false, ipaddress, out uintIpAddress);
            return (uint)uintIpAddress;

        }

        /// <summary>
        /// Convert an ipadress to decimal
        /// 0.0.0.0 -> 0
        /// 0.0.1.0 -> 256
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static bool TryToUint(IPAddress ipaddress, out uint? uintIpAddress) {
            uint? uintIpAddress2 = null;
            IPNetwork.InternalToUint(true, ipaddress, out uintIpAddress2);
            bool parsed = (uintIpAddress2 != null);
            uintIpAddress = uintIpAddress2;
            return parsed;
        }

        private static void InternalToUint(bool tryParse, IPAddress ipaddress, out uint? uintIpAddress) {

            if (ipaddress == null) {
                if (tryParse == false) {
                    throw new ArgumentNullException("ipaddress");
                }
                uintIpAddress = null;
                return;
            }

            byte[] bytes = ipaddress.GetAddressBytes();
            if (bytes.Length != 4) {
                if (tryParse == false) {
                    throw new ArgumentException("bytes");
                }
                uintIpAddress = null;
                return;

            }

            Array.Reverse(bytes);
            uint value = BitConverter.ToUInt32(bytes, 0);
            uintIpAddress = value;
            return;
        }


        /// <summary>
        /// Convert a cidr to uint netmask
        /// </summary>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static uint ToUint(byte cidr) {

            uint? uintNetmask = null;
            IPNetwork.InternalToUint(false, cidr, out uintNetmask);
            return (uint)uintNetmask;
        }


        /// <summary>
        /// Convert a cidr to uint netmask
        /// </summary>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool TryToUint(byte cidr, out uint? uintNetmask) {

            uint? uintNetmask2 = null;
            IPNetwork.InternalToUint(true, cidr, out uintNetmask2);
            bool parsed = (uintNetmask2 != null);
            uintNetmask = uintNetmask2;
            return parsed;
        }

        /// <summary>
        /// Convert a cidr to uint netmask
        /// </summary>
        /// <param name="cidr"></param>
        /// <returns></returns>
        private static void InternalToUint(bool tryParse, byte cidr, out uint? uintNetmask) {
            if (cidr > 32) {
                if (tryParse == false) {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                uintNetmask = null;
                return;
            }
            uint uintNetmask2 = cidr == 0 ? 0 : 0xffffffff << (32 - cidr);
            uintNetmask = uintNetmask2;
        }

        #endregion

        #region ToCidr
        /// <summary>
        /// Convert netmask to CIDR
        ///  255.255.255.0 -> 24
        ///  255.255.0.0   -> 16
        ///  255.0.0.0     -> 8
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private static byte ToCidr(uint netmask) {
            byte? cidr = null;
            IPNetwork.InternalToCidr(false, netmask, out cidr);
            return (byte)cidr;
        }

        /// <summary>
        /// Convert netmask to CIDR
        ///  255.255.255.0 -> 24
        ///  255.255.0.0   -> 16
        ///  255.0.0.0     -> 8
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private static void InternalToCidr(bool tryParse, uint netmask, out byte? cidr) {

            if (!IPNetwork.ValidNetmask(netmask)) {
                if (tryParse == false) {
                    throw new ArgumentException("netmask");
                }
                cidr = null;
                return;
            }

            byte cidr2 = IPNetwork.BitsSet(netmask);
            cidr = cidr2;
            return;

        }
        /// <summary>
        /// Convert netmask to CIDR
        ///  255.255.255.0 -> 24
        ///  255.255.0.0   -> 16
        ///  255.0.0.0     -> 8
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static byte ToCidr(IPAddress netmask) {
            byte? cidr = null;
            IPNetwork.InternalToCidr(false, netmask, out cidr);
            return (byte)cidr;
        }

        /// <summary>
        /// Convert netmask to CIDR
        ///  255.255.255.0 -> 24
        ///  255.255.0.0   -> 16
        ///  255.0.0.0     -> 8
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static bool TryToCidr(IPAddress netmask, out byte? cidr) {
            byte? cidr2 = null;
            IPNetwork.InternalToCidr(true, netmask, out cidr2);
            bool parsed = (cidr2 != null);
            cidr = cidr2;
            return parsed;
        }

        private static void InternalToCidr(bool tryParse, IPAddress netmask, out byte? cidr) {

            if (netmask == null) {
                if (tryParse == false) {
                    throw new ArgumentNullException("netmask");
                }
                cidr = null;
                return;
            }
            uint? uintNetmask2 = null;
            bool parsed = IPNetwork.TryToUint(netmask, out uintNetmask2);
            if (parsed == false) {
                if (tryParse == false) {
                    throw new ArgumentException("netmask");
                }
                cidr = null;
                return;
            }
            uint uintNetmask = (uint)uintNetmask2;

            byte? cidr2 = null;
            IPNetwork.InternalToCidr(tryParse, uintNetmask, out cidr2);
            cidr = cidr2;

            return;

        }


        #endregion

        #region ToNetmask

        /// <summary>
        /// Convert CIDR to netmask
        ///  24 -> 255.255.255.0
        ///  16 -> 255.255.0.0
        ///  8 -> 255.0.0.0
        /// </summary>
        /// <see cref="http://snipplr.com/view/15557/cidr-class-for-ipv4/"/>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static IPAddress ToNetmask(byte cidr) {

            IPAddress netmask = null;
            IPNetwork.InternalToNetmask(false, cidr, out netmask);
            return netmask;
        }

        /// <summary>
        /// Convert CIDR to netmask
        ///  24 -> 255.255.255.0
        ///  16 -> 255.255.0.0
        ///  8 -> 255.0.0.0
        /// </summary>
        /// <see cref="http://snipplr.com/view/15557/cidr-class-for-ipv4/"/>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool TryToNetmask(byte cidr, out IPAddress netmask) {

            IPAddress netmask2 = null;
            IPNetwork.InternalToNetmask(true, cidr, out netmask2);
            bool parsed = (netmask2 != null);
            netmask = netmask2;
            return parsed;
        }


        private static void InternalToNetmask(bool tryParse, byte cidr, out IPAddress netmask) {
            if (cidr < 0 || cidr > 32) {
                if (tryParse == false) {
                    throw new ArgumentOutOfRangeException("cidr");
                }
                netmask = null;
                return;
            }
            uint mask = IPNetwork.ToUint(cidr);
            IPAddress netmask2 = IPNetwork.ToIPAddress(mask);
            netmask = netmask2;

            return;
        }

        #endregion

        #endregion

        #region utils

        #region BitsSet

        /// <summary>
        /// Count bits set to 1 in netmask
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/109023/best-algorithm-to-count-the-number-of-set-bits-in-a-32-bit-integer"/>
        /// <param name="netmask"></param>
        /// <returns></returns>
        private static byte BitsSet(uint netmask) {
            uint i = netmask;
            i = i - ((i >> 1) & 0x55555555);
            i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
            i = ((i + (i >> 4) & 0xf0f0f0f) * 0x1010101) >> 24;
            return (byte)i;
        }

        /// <summary>
        /// Count bits set to 1 in netmask
        /// </summary>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static byte BitsSet(IPAddress netmask) {
            uint uintNetmask = IPNetwork.ToUint(netmask);
            byte bits = IPNetwork.BitsSet(uintNetmask);
            return bits;
        }

        #endregion

        #region ValidNetmask

        /// <summary>
        /// return true if netmask is a valid netmask
        /// 255.255.255.0, 255.0.0.0, 255.255.240.0, ...
        /// </summary>
        /// <see cref="http://www.actionsnip.com/snippets/tomo_atlacatl/calculate-if-a-netmask-is-valid--as2-"/>
        /// <param name="netmask"></param>
        /// <returns></returns>
        public static bool ValidNetmask(IPAddress netmask) {

            if (netmask == null) {
                throw new ArgumentNullException("netmask");
            }
            uint uintNetmask = IPNetwork.ToUint(netmask);
            bool valid = IPNetwork.ValidNetmask(uintNetmask);
            return valid;
        }

        private static bool ValidNetmask(uint netmask) {
            long neg = ((~(int)netmask) & 0xffffffff);
            bool isNetmask = ((neg + 1) & neg) == 0;
            return isNetmask;
        }

        #endregion 

        #region ToIPAddress

        /// <summary>
        /// Transform a uint ipaddress into IPAddress object
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static IPAddress ToIPAddress(uint ipaddress) {
            byte[] bytes = BitConverter.GetBytes(ipaddress);
            Array.Reverse(bytes);
            IPAddress ip = new IPAddress(bytes);
            return ip;
        }

        #endregion

        #endregion

        #region contains

        /// <summary>
        /// return true if ipaddress is contained in network
        /// </summary>
        /// <param name="network"></param>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static bool Contains(IPNetwork network, IPAddress ipaddress) {

            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (ipaddress == null)
            {
                throw new ArgumentNullException("ipaddress");
            }

            uint uintNetwork = network._network;
            uint uintBroadcast = network._broadcast;
            uint uintAddress = IPNetwork.ToUint(ipaddress);

            bool contains = (uintAddress >= uintNetwork
                && uintAddress <= uintBroadcast);

            return contains;
            
        }

        /// <summary>
        /// return true is network2 is fully contained in network
        /// </summary>
        /// <param name="network"></param>
        /// <param name="network2"></param>
        /// <returns></returns>
        public static bool Contains(IPNetwork network, IPNetwork network2) {

            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (network2 == null)
            {
                throw new ArgumentNullException("network2");
            }
            
            uint uintNetwork = network._network;
            uint uintBroadcast = network._broadcast;

            uint uintFirst = network2._network;
            uint uintLast = network2._broadcast;

            bool contains = (uintFirst >= uintNetwork
                && uintLast <= uintBroadcast);

            return contains;
        }

        #endregion

        #region overlap

        /// <summary>
        /// return true is network2 overlap network
        /// </summary>
        /// <param name="network"></param>
        /// <param name="network2"></param>
        /// <returns></returns>
        public static bool Overlap(IPNetwork network, IPNetwork network2) {

            if (network == null)
            {
                throw new ArgumentNullException("network");
            }

            if (network2 == null)
            {
                throw new ArgumentNullException("network2");
            }
            

            uint uintNetwork = network._network;
            uint uintBroadcast = network._broadcast;

            uint uintFirst = network2._network;
            uint uintLast = network2._broadcast;

            bool overlap =
                (uintFirst >= uintNetwork && uintFirst <= uintBroadcast)
                || (uintLast >= uintNetwork && uintLast <= uintBroadcast)
                || (uintFirst <= uintNetwork && uintLast >= uintBroadcast)
                || (uintFirst >= uintNetwork && uintLast <= uintBroadcast);

            return overlap;
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return string.Format("{0}/{1}", this.Network, this.Cidr);
        }

        #endregion

        #region IANA block

        private static IPNetwork _iana_ablock_reserved = IPNetwork.Parse("10.0.0.0/8");
        private static IPNetwork _iana_bblock_reserved = IPNetwork.Parse("172.16.0.0/12");
        private static IPNetwork _iana_cblock_reserved = IPNetwork.Parse("192.168.0.0/16");

        /// <summary>
        /// 10.0.0.0/8
        /// </summary>
        /// <returns></returns>
        public static IPNetwork IANA_ABLK_RESERVED1 {
            get {
                return IPNetwork._iana_ablock_reserved;
            }
        }

        /// <summary>
        /// 172.12.0.0/12
        /// </summary>
        /// <returns></returns>
        public static IPNetwork IANA_BBLK_RESERVED1 {
            get {
                return IPNetwork._iana_bblock_reserved;
            }
        }

        /// <summary>
        /// 192.168.0.0/16
        /// </summary>
        /// <returns></returns>
        public static IPNetwork IANA_CBLK_RESERVED1 {
            get {
                return IPNetwork._iana_cblock_reserved;
            }
        }

        /// <summary>
        /// return true if ipaddress is contained in 
        /// IANA_ABLK_RESERVED1, IANA_BBLK_RESERVED1, IANA_CBLK_RESERVED1
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static bool IsIANAReserved(IPAddress ipaddress) {

            if (ipaddress == null) {
                throw new ArgumentNullException("ipaddress");
            }

            return IPNetwork.Contains(IPNetwork.IANA_ABLK_RESERVED1, ipaddress)
                || IPNetwork.Contains(IPNetwork.IANA_BBLK_RESERVED1, ipaddress)
                || IPNetwork.Contains(IPNetwork.IANA_CBLK_RESERVED1, ipaddress);
        }

        /// <summary>
        /// return true if ipnetwork is contained in 
        /// IANA_ABLK_RESERVED1, IANA_BBLK_RESERVED1, IANA_CBLK_RESERVED1
        /// </summary>
        /// <param name="ipnetwork"></param>
        /// <returns></returns>
        public static bool IsIANAReserved(IPNetwork ipnetwork) {

            if (ipnetwork == null) {
                throw new ArgumentNullException("ipnetwork");
            }

            return IPNetwork.Contains(IPNetwork.IANA_ABLK_RESERVED1, ipnetwork)
                || IPNetwork.Contains(IPNetwork.IANA_BBLK_RESERVED1, ipnetwork)
                || IPNetwork.Contains(IPNetwork.IANA_CBLK_RESERVED1, ipnetwork);
        }

        #endregion

        #region Equals

        public override bool Equals(object obj) {

            if (obj == null) {
                return false;
            }

            if (!(obj is IPNetwork)) {
                return false;
            }

            IPNetwork remote = (IPNetwork)obj;
            if (this._network != remote._network) {
                return false;
            }

            if (this._cidr != remote._cidr) {
                return false;
            }

            return true;
        }

        #endregion

        #region GetHashCode

        public override int GetHashCode() {
            return string.Format("{0}|{1}|{2}",
                this._ipaddress.GetHashCode(),
                this._network.GetHashCode(),
                this._cidr.GetHashCode()).GetHashCode();
        }

        #endregion

        

        

        #region Print
        /// <summary>
        /// Print an ipnetwork in a clear representation string
        /// </summary>
        /// <param name="ipnetwork"></param>
        /// <returns></returns>
        public static string Print(IPNetwork ipnetwork) {

            if (ipnetwork == null) {
                throw new ArgumentNullException("ipnetwork");
            }
            StringWriter sw = new StringWriter();

            sw.WriteLine("IPNetwork   : {0}", ipnetwork.ToString());
            sw.WriteLine("Network     : {0}", ipnetwork.Network);
            sw.WriteLine("Netmask     : {0}", ipnetwork.Netmask);
            sw.WriteLine("Cidr        : {0}", ipnetwork.Cidr);
            sw.WriteLine("Broadcast   : {0}", ipnetwork.Broadcast);
            sw.WriteLine("FirstUsable : {0}", ipnetwork.FirstUsable);
            sw.WriteLine("LastUsable  : {0}", ipnetwork.LastUsable);
            sw.WriteLine("Usable      : {0}", ipnetwork.Usable);

            return sw.ToString();
        }

        #endregion

        #region TryGuessCidr

        /// <summary>
        /// 
        /// Class              Leading bits    Default netmask
        ///     A (CIDR /8)	       00           255.0.0.0
        ///     A (CIDR /8)	       01           255.0.0.0
        ///     B (CIDR /16)	   10           255.255.0.0
        ///     C (CIDR /24)       11 	        255.255.255.0
        ///  
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool TryGuessCidr(string ip, out byte cidr) {

            IPAddress ipaddress = null;
            bool parsed = IPAddress.TryParse(string.Format("{0}", ip), out ipaddress);
            if (parsed == false) {
                cidr = 0;
                return false;
            }
            uint uintIPAddress = IPNetwork.ToUint(ipaddress);
            uintIPAddress = uintIPAddress >> 29;
            if (uintIPAddress <= 3) {
                cidr = 8;
                return true;
            } else if (uintIPAddress <= 5) {
                cidr = 16;
                return true;
            } else if (uintIPAddress <= 6) {
                cidr = 24;
                return true;
            }

            cidr = 0;
            return false;

        }

        /// <summary>
        /// Try to parse cidr. Have to be >= 0 and <= 32
        /// </summary>
        /// <param name="sidr"></param>
        /// <param name="cidr"></param>
        /// <returns></returns>
        public static bool TryParseCidr(string sidr, out byte? cidr) {

            byte b = 0;
            if (!byte.TryParse(sidr, out b)) {
                cidr = null;
                return false;
            }

            IPAddress netmask = null;
            if (!IPNetwork.TryToNetmask(b, out netmask)) {
                cidr = null;
                return false;
            }

            cidr = b;
            return true;
        }

        #endregion

     
        #region IComparable<IPNetwork> Members

        public int CompareTo(IPNetwork other) {
            int network = this._network.CompareTo(other._network);
            if (network != 0) {
                return network;
            }

            int cidr = this._cidr.CompareTo(other._cidr);
            return cidr;
        }

        #endregion
    }
}