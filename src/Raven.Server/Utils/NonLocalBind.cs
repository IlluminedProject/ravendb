using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;

namespace Raven.Server.Utils
{
    internal static class NonLocalBind
    {
        public static void Enable(Socket socket)
        {
            if (OperatingSystem.IsLinux() == false)
                return;

            int level;
            int option;
            if (socket.AddressFamily == AddressFamily.InterNetwork)
            {
                level = 0; // SOL_IP
                option = 15; // IP_FREEBIND
            }
            else if (socket.AddressFamily == AddressFamily.InterNetworkV6)
            {
                level = 41; // SOL_IPV6
                option = 78; // IPV6_FREEBIND
            }
            else
                return;

            int enabled = 1;
            if (setsockopt(socket.SafeHandle, level, option, ref enabled, sizeof(int)) != 0)
                throw new SocketException(Marshal.GetLastPInvokeError());
        }

        public static Socket CreateBoundListenSocket(EndPoint endpoint)
        {
            if (OperatingSystem.IsLinux() == false || endpoint is not IPEndPoint ip)
                return SocketTransportOptions.CreateDefaultBoundListenSocket(endpoint);

            var socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                if (ip.Address.Equals(IPAddress.IPv6Any))
                    socket.DualMode = true;

                Enable(socket);
                socket.Bind(endpoint);
                return socket;
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int setsockopt(SafeSocketHandle socket, int level, int option, ref int value, uint length);
    }
}
