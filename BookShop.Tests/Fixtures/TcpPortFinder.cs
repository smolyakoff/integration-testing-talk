using System.Net;
using System.Net.Sockets;

namespace BookShop.Tests.Fixtures
{
    internal static class TcpPortFinder
    {
        public static ushort FindAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return (ushort)((IPEndPoint) listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}