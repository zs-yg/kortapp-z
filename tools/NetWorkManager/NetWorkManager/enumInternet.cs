using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Diagnostics;
namespace NetWorkManager
{


public class PortScanner
{

    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedTcpTable(
        IntPtr pTcpTable,
        ref int dwOutBufLen,
        bool sort,
        int ipVersion,
        TCP_TABLE_CLASS tblClass,
        uint reserved = 0);

    private enum TCP_TABLE_CLASS
    {
        TCP_TABLE_BASIC_LISTENER,
        TCP_TABLE_BASIC_CONNECTIONS,
        TCP_TABLE_BASIC_ALL,
        TCP_TABLE_OWNER_PID_LISTENER,
        TCP_TABLE_OWNER_PID_CONNECTIONS,
        TCP_TABLE_OWNER_PID_ALL,
        TCP_TABLE_OWNER_MODULE_LISTENER,
        TCP_TABLE_OWNER_MODULE_CONNECTIONS,
        TCP_TABLE_OWNER_MODULE_ALL
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPROW_OWNER_PID
    {
        public uint state;
        public uint localAddr;
        public uint localPort;
        public uint remoteAddr;
        public uint remotePort;
        public uint owningPid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCPROW_OWNER_PID[] table;
    }
    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddr;
        public uint localScopeId;
        public uint localPort;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] remoteAddr;
        public uint remoteScopeId;
        public uint remotePort;
        public uint state;
        public uint owningPid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_TCP6TABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_TCP6ROW_OWNER_PID[] table;
    }
    private enum UDP_TABLE_CLASS
    {
        UDP_TABLE_BASIC,
        UDP_TABLE_OWNER_PID,
        UDP_TABLE_OWNER_MODULE
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDP6ROW_OWNER_PID
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] localAddr;
        public uint localScopeId;
        public uint localPort;
        public uint owningPid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDP6TABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_UDP6ROW_OWNER_PID[] table;
    }




    [DllImport("iphlpapi.dll", SetLastError = true)]
    private static extern uint GetExtendedUdpTable(
        IntPtr pUdpTable,
        ref int dwOutBufLen,
        bool sort,
        int ipVersion,
        UDP_TABLE_CLASS tblClass,
        uint reserved = 0);



    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDPROW_OWNER_PID
    {
        public uint localAddr;
        public uint localPort;
        public uint owningPid;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MIB_UDPTABLE_OWNER_PID
    {
        public uint dwNumEntries;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.Struct, SizeConst = 1)]
        public MIB_UDPROW_OWNER_PID[] table;
    }


    private static string[] TcpStateStrings = new string[] {
        "CLOSED", "LISTENING", "SYN_SENT", "SYN_RCVD",
        "ESTABLISHED", "FIN_WAIT1", "FIN_WAIT2", "CLOSE_WAIT",
        "CLOSING", "LAST_ACK", "TIME_WAIT", "DELETE_TCB"
    };
    private const int AF_INET = 2;
    private const int AF_INET6 = 23;

   public class internet_info
    {
        public string pid;
        public string process;
        public string local;
        public string remote;
        public string state;
        public string protocal;

       public internet_info(string pid, string process, string local, string remote, string state, string protocal)
        {
            this.pid = pid.Trim();
            this.process = process.Trim();
            this.local = local.Trim();
            this.remote = remote.Trim();
            this.state = state.Trim();
            this.protocal = protocal.Trim();
        }   
    }
   public static List<internet_info> internet_infolist = new List<internet_info>();
    public  static List<internet_info> GetAllInfo()
    {
        internet_infolist.Clear();

        Console.WriteLine("IPv4 TCP Connections:");
        ScanTcpPorts(AF_INET);
        Console.WriteLine("\nIPv6 TCP Connections:");
        ScanTcpPorts(AF_INET6);
        Console.WriteLine("\nIPv4 UDP Connections:");
        ScanUdpPorts(AF_INET);
        Console.WriteLine("\nIPv6 UDP Connections:");
        ScanUdpPorts(AF_INET6);

        return internet_infolist;
       
    }

    private static void ScanTcpPorts(int ipVersion)
    {
        IntPtr tcpTable = IntPtr.Zero;
        int tcpTableSize = 0;
        uint ret = GetExtendedTcpTable(
            tcpTable,
            ref tcpTableSize,
            true,
            ipVersion,
            TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

        if (ret != 0 && ret != 122) // ERROR_INSUFFICIENT_BUFFER
        {
            Console.WriteLine($"GetExtendedTcpTable failed: {ret}");
            return;
        }

        tcpTable = Marshal.AllocHGlobal(tcpTableSize);
        ret = GetExtendedTcpTable(
            tcpTable,
            ref tcpTableSize,
            true,
            ipVersion,
            TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);

        if (ret != 0)
        {
            Console.WriteLine($"GetExtendedTcpTable failed: {ret}");
            Marshal.FreeHGlobal(tcpTable);
            return;
        }

        if (ipVersion == AF_INET)
        {

            var table = Marshal.PtrToStructure<MIB_TCPTABLE_OWNER_PID>(tcpTable);
            IntPtr rowPtr = (IntPtr)((long)tcpTable + Marshal.SizeOf(table.dwNumEntries));

            for (int i = 0; i < table.dwNumEntries; i++)
            {
                var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(rowPtr);
                string processName = GetProcessName((int)row.owningPid);
                string localAddr = new IPAddress(row.localAddr).ToString();
                string remoteAddr = row.remoteAddr != 0 ?
                    new IPAddress(row.remoteAddr).ToString() : "0.0.0.0";
                ushort localPort = NetworkToHostPort(row.localPort);
                ushort remotePort = NetworkToHostPort(row.remotePort);

                Console.WriteLine(
                    $"PID: {row.owningPid,6} | " +
                    $"Process: {processName,-20} | " +
                    $"Local: {localAddr}:{localPort,-5} | " +
                    $"Remote: {remoteAddr}:{remotePort,-5} | " +
                    $"State: {TcpStateStrings[row.state]}");

                rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID)));

                internet_info ifo = new internet_info($"{row.owningPid,6}", $"{processName,-20}", $"{localAddr}:{localPort,-5}", $"{remoteAddr}:{remotePort,-5}", $"{TcpStateStrings[row.state]}", "tcpv4");

                internet_infolist.Add(ifo);
            }
        }
        else
        {

            var table = Marshal.PtrToStructure<MIB_TCP6TABLE_OWNER_PID>(tcpTable);
            IntPtr rowPtr = (IntPtr)((long)tcpTable + Marshal.SizeOf(table.dwNumEntries));

            for (int i = 0; i < table.dwNumEntries; i++)
            {
                var row = Marshal.PtrToStructure<MIB_TCP6ROW_OWNER_PID>(rowPtr);
                string processName = GetProcessName((int)row.owningPid);
                IPAddress localAddr = new IPAddress(row.localAddr, row.localScopeId);
                IPAddress remoteAddr = new IPAddress(row.remoteAddr, row.remoteScopeId);
                ushort localPort = NetworkToHostPort(row.localPort);
                ushort remotePort = NetworkToHostPort(row.remotePort);

                Console.WriteLine(
                    $"PID: {row.owningPid,6} | " +
                    $"Process: {processName,-20} | " +
                    $"Local: [{localAddr}]:{localPort,-5} | " +
                    $"Remote: [{remoteAddr}]:{remotePort,-5} | " +
                    $"State: {TcpStateStrings[row.state]}");

                rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(MIB_TCP6ROW_OWNER_PID)));

                internet_info ifo = new internet_info($"{row.owningPid,6}", $"{processName,-20}", $"{localAddr}:{localPort,-5}", $"{remoteAddr}:{remotePort,-5}", $"{TcpStateStrings[row.state]}", "tcpv6");

                internet_infolist.Add(ifo);
            }
        }

        Marshal.FreeHGlobal(tcpTable);
    }

    private static void ScanUdpPorts(int ipVersion)
    {
        IntPtr udpTable = IntPtr.Zero;
        int udpTableSize = 0;
        uint ret = GetExtendedUdpTable(
            udpTable,
            ref udpTableSize,
            true,
            ipVersion,
            UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);

        if (ret != 0 && ret != 122) // ERROR_INSUFFICIENT_BUFFER
        {
            Console.WriteLine($"GetExtendedUdpTable failed: {ret}");
            return;
        }

        udpTable = Marshal.AllocHGlobal(udpTableSize);
        ret = GetExtendedUdpTable(
            udpTable,
            ref udpTableSize,
            true,
            ipVersion,
            UDP_TABLE_CLASS.UDP_TABLE_OWNER_PID);

        if (ret != 0)
        {
            Console.WriteLine($"GetExtendedUdpTable failed: {ret}");
            Marshal.FreeHGlobal(udpTable);
            return;
        }

        if (ipVersion == AF_INET)
        {

            var table = Marshal.PtrToStructure<MIB_UDPTABLE_OWNER_PID>(udpTable);
            IntPtr rowPtr = (IntPtr)((long)udpTable + Marshal.SizeOf(table.dwNumEntries));

            for (int i = 0; i < table.dwNumEntries; i++)
            {
                var row = Marshal.PtrToStructure<MIB_UDPROW_OWNER_PID>(rowPtr);
                string processName = GetProcessName((int)row.owningPid);
                string localAddr = new IPAddress(row.localAddr).ToString();
                ushort localPort = NetworkToHostPort(row.localPort);

                Console.WriteLine(
                    $"PID: {row.owningPid,6} | " +
                    $"Process: {processName,-20} | " +
                    $"Local: {localAddr}:{localPort} | " +
                    $"State: LISTENING");

                rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(MIB_UDPROW_OWNER_PID)));

                    internet_info ifo = new internet_info($"{row.owningPid,6}", $"{processName,-20}", $"{localAddr}:{localPort,-5}", $"", $"", "udpv4");

                    internet_infolist.Add(ifo);
                }
        }
        else
        {

            var table = Marshal.PtrToStructure<MIB_UDP6TABLE_OWNER_PID>(udpTable);
            IntPtr rowPtr = (IntPtr)((long)udpTable + Marshal.SizeOf(table.dwNumEntries));

            for (int i = 0; i < table.dwNumEntries; i++)
            {
                var row = Marshal.PtrToStructure<MIB_UDP6ROW_OWNER_PID>(rowPtr);
                string processName = GetProcessName((int)row.owningPid);
                IPAddress localAddr = new IPAddress(row.localAddr, row.localScopeId);
                ushort localPort = NetworkToHostPort(row.localPort);

                Console.WriteLine(
                    $"PID: {row.owningPid,6} | " +
                    $"Process: {processName,-20} | " +
                    $"Local: [{localAddr}]:{localPort} | " +
                    $"State: LISTENING");

                rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(typeof(MIB_UDP6ROW_OWNER_PID)));

                    internet_info ifo = new internet_info($"{row.owningPid,6}", $"{processName,-20}", $"{localAddr}:{localPort,-5}", $"", $"", "udpv6");

                    internet_infolist.Add(ifo);
                }
        }

        Marshal.FreeHGlobal(udpTable);
    }


    private static ushort NetworkToHostPort(uint port)
    {
        byte[] bytes = BitConverter.GetBytes(port);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes, 0, 2);
        }
        return BitConverter.ToUInt16(bytes, 0);
    }

    private static string GetProcessName(int pid)
    {
        try
        {
            Process process = Process.GetProcessById(pid);
            return process.ProcessName;
        }
        catch
        {
            return "N/A";
        }
    }
}

}