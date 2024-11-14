using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tanks_Sever.tanks.auth;
using Tanks_Sever.tanks.lobby;
using Tanks_Sever.tanks.main.procotol.commands;

namespace Tanks_Sever.tanks.main.procotol
{
    public class ProtocolTransfer
    {
        private String SPLITTER_CMDS = "end~";
        private StringBuilder inputRequest = new StringBuilder();
        private StringBuilder badRequest = new StringBuilder();
        public LobbyManager lobby;
        private Auth auth;
        private int _lastKey = 1;
        private int[] _keys = {1, 2, 3, 4, 5, 6, 7, 8, 9};
        private TcpClient channel;

        public void Init(TcpClient client) {
            channel = client;
        }

        public void DecryptProtocol(string protocol)
        {
            IPEndPoint remoteEndPoint = channel.Client.RemoteEndPoint as IPEndPoint;
            if (inputRequest.Append(protocol).Length > 0)
            {
                if (inputRequest.ToString().EndsWith(SPLITTER_CMDS))
                {
                    inputRequest.Append(badRequest.ToString());

                    //Console.WriteLine("содержимое переменной inputRequest: " + inputRequest.ToString());

                    string[] requests = ParseCryptRequests();

                    foreach (string request in requests)
                    {
                        //Console.WriteLine(request);

                        int key;
                        try
                        {
                            key = int.Parse(request[0].ToString());
                        }
                        catch (Exception)
                        {
                            Console.WriteLine($"[EXCEPTION] Detected cheater (replace protocol): {remoteEndPoint.Address}:{remoteEndPoint.Port}");
                            CloseConnection();
                            return;
                        }

                        if (key == _lastKey)
                        {
                            Console.WriteLine($"Detected cheater (replace protocol): {remoteEndPoint.Address}:{remoteEndPoint.Port}");
                            CloseConnection();
                            return;
                        }

                        int nextKey = (_lastKey + 1) % _keys.Length;
                        if (key != (nextKey == 0 ? 1 : nextKey))
                        {
                            Console.WriteLine($"[NOT QUEUE KEY {nextKey}] Detected cheater (replace protocol): {remoteEndPoint.Address}:{remoteEndPoint.Port}");
                            CloseConnection();
                            return;
                        }

                        string decryptedRequest = Decrypt(request.Substring(1), key);
                        SendRequestToManagers(decryptedRequest);

                        _lastKey = key;
                        int endIndex = inputRequest.ToString().IndexOf(SPLITTER_CMDS) + SPLITTER_CMDS.Length;
                        inputRequest.Remove(0, endIndex);
                    }

                    badRequest.Clear();
                }
                else
                {
                    badRequest.Append(protocol);
                }
            }
        }


        private string Decrypt(string request, int key)
        {
            _lastKey = key;
            char[] chars = request.ToCharArray();

            for (int i = 0; i < request.Length; i++)
            {
                chars[i] = (char)(chars[i] - key);
            }

            Console.WriteLine(chars);

            return new string(chars);
        }

        private string[] ParseCryptRequests()
        {
            return inputRequest.ToString().Split(new[] { SPLITTER_CMDS }, StringSplitOptions.RemoveEmptyEntries);
        }

        private void SendRequestToManagers(string request)
        {
            try
            {
                var command = Commands.Decrypt(request);
                SendCommandToManagers(command);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error send To Managers: {ex.Message}");
            }
        }

        private void SendCommandToManagers(Command command)
        {
            if (auth == null)
            {
                auth = new Auth(this);
            }

            switch (command.Type)
            {
                case CommandType.AUTH:
                    auth.ExecuteCommand(command);
                    break;
                case CommandType.REGISTRATION:
                    auth.ExecuteCommand(command);
                    break;
                case CommandType.GARAGE:
                    lobby.ExecuteCommand(command);
                    break;
                case CommandType.CHAT:
                    lobby.ExecuteCommand(command);
                    break;
                case CommandType.LOBBY:
                    lobby.ExecuteCommand(command);
                    break;
                case CommandType.LOBBY_CHAT:
                    lobby.ExecuteCommand(command);
                    break;
                case CommandType.BATTLE:
                    lobby.ExecuteCommand(command);
                    break;
                case CommandType.PING:
                    auth.ExecuteCommand(command);
                    break;
                case CommandType.UNKNOWN:
                    Console.WriteLine($"User {GetIP()} sent unknown request: {command}");
                    break;
                case CommandType.SYSTEM:
                    if (auth != null)
                    {
                        auth.ExecuteCommand(command);
                    }

                    if (lobby != null) 
                    {
                        lobby.ExecuteCommand(command);
                    }
                    break;
                default:
                    break;
            }
        }

        public bool Send(CommandType type, params string[] args)
        {
            StringBuilder request = new StringBuilder();
            request.Append(type.ToStringValue());
            request.Append(";");

            for (int i = 0; i < args.Length - 1; i++)
            {
                request.Append(args[i]);
                request.Append(";");
            }

            request.Append(args[args.Length - 1]);
            request.Append(SPLITTER_CMDS);

            try
            {
                if (channel != null && channel.Connected)
                {
                    NetworkStream stream = channel.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(request.ToString());
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Send: " + request.ToString());
                    return true;
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Error while sending data: " + e.Message);
            }

            return false;
        }

        public void OnDisconnect()
        { 
            if (lobby != null)
            {
                lobby.OnDisconnect();
            }
        }

        public void CloseConnection() 
        {
            if (channel != null) 
            {

                channel.Close();
            }
        }

        public string GetIP()
        {
            return channel.Client.RemoteEndPoint.ToString();
        }

    }
}
