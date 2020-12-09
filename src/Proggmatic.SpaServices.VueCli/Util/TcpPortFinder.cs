using System.Net;
using System.Net.Sockets;


namespace Proggmatic.SpaServices.VueCli.Util
{
    /// <summary>
    /// Original: https://github.com/dotnet/aspnetcore/blob/master/src/Middleware/SpaServices.Extensions/src/Util/TcpPortFinder.cs
    /// </summary>
    internal static class TcpPortFinder
    {
        public static int FindAvailablePort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            try
            {
                return ((IPEndPoint)listener.LocalEndpoint).Port;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}