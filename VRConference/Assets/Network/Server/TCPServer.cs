using System;
using System.Net;
using System.Net.Sockets;
using Network.Both;
using UnityEngine;

namespace Network.Server.Code
{
    public class TCPServer
    {
        private readonly Server server;
        public TCPServer(Server server)
        {
            this.server = server;
        }
        
        private TcpListener tcpListener;

        public void Start()
        {
            if (server.serverState != NetworkState.connecting) { return; }
            
            tcpListener = new TcpListener(IPAddress.Any, server.port.value);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
        }
        
        private void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            
            Debug.Log($"SERVER: Incoming connection from {tcpClient.Client.RemoteEndPoint}...");

            for (byte i = 1; i <= State.MaxClients; i++)
            {
                if (server.clients[i] != null) continue;

                ServerClient client = new ServerClient(i, tcpClient);
                server.clients[i] = client;
                server.ConnectClient(client);
                return;
            }

            Debug.Log($"SERVER: {tcpClient.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        public void ConnectClient(ServerClient client)
        {
            if (server.serverState != NetworkState.connected) { return; }
            
            client.socket.ReceiveBufferSize = State.BufferSize;
            client.socket.SendBufferSize = State.BufferSize;
            client.stream = client.socket.GetStream();
            client.ip =  client.socket.Client.RemoteEndPoint.ToString().Split(':')[0];

            client.receiveBuffer = new byte[State.BufferSize];
            client.stream.BeginRead(client.receiveBuffer, 0, State.BufferSize, ReceiveCallback, client);
            
            server.networkSend.UserStatus(1);
            server.serverSend.ServerClientId(client);
        }
        
        private void ReceiveCallback(IAsyncResult result)
        {
            if (server.serverState != NetworkState.connected) { return; }

            ServerClient client = (ServerClient) result.AsyncState;
            try
            {
                int byteLength = client.stream.EndRead(result);
                if (byteLength < State.HeaderSize)
                {
                    server.DisconnectClient(client);
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(client.receiveBuffer, data, byteLength);
                server.HandelData(data);
                
                client.stream.BeginRead(client.receiveBuffer, 0, State.BufferSize, ReceiveCallback, client);
            }
            catch
            {
                server.DisconnectClient(client);
            }
        }
        
        public void SendData(ServerClient client, byte[] data, int length)
        {
            if (server.serverState != NetworkState.connected) { return; }

            try
            {
                client.stream.BeginWrite(data, 0, length, null, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to player {client.id} via TCP: {ex}");
            }
        }

        public void DisconnectClient(ServerClient client)
        {
            if (client.socket != null)
            {
                client.socket.Close();
            }
            client.stream = null;
            client.receiveBuffer = null;
            client.socket = null;
        }

        public void Stop()
        {
            tcpListener.Stop();
        }
    }
}