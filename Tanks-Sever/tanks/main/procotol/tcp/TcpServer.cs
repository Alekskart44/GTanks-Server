using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.loaders;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.main.procotol.tcp
{
    internal class TcpServer
    {
        private TcpListener _server;
        private static readonly TcpServer instance = new TcpServer();

        public static TcpServer Inject()
        {
            return instance;
        }

        public void Init()
        {
            int port = ConfiguratorLoader.Port;
            _server = new TcpListener(IPAddress.Any, port);
            _server.Start();
            Console.WriteLine($"[TCP] Server started on port: {port}");
            AcceptClientsAsync();
        }

        private async void AcceptClientsAsync()
        {
            while (true)
            {
                TcpClient client = await _server.AcceptTcpClientAsync();
                HandleClientAsync(client);
            }
        }

        private async void HandleClientAsync(TcpClient client)
        {
            ProtocolTransfer protocolTransfer = new ProtocolTransfer();
            IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            Console.WriteLine($"Client connected: {remoteEndPoint.Address}:{remoteEndPoint.Port}");
            protocolTransfer.Init(client);

            using (client)
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                try
                {
                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        protocolTransfer.DecryptProtocol(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    CloseConnection(client, protocolTransfer, remoteEndPoint);
                }
            }
        }

        private void CloseConnection(TcpClient client, ProtocolTransfer protocolTransfer, IPEndPoint remoteEndPoint)
        {
            protocolTransfer.OnDisconnect();
            protocolTransfer.CloseConnection();
            Console.WriteLine($"Connection closed: {remoteEndPoint.Address}:{remoteEndPoint.Port}");
        }

    }
}
