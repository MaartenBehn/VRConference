using System;
using System.Net;
using System.Net.Sockets;
using Engine;
using UnityEngine;
using Users;
using Utility;

namespace Network.Server
{
    public class TCPServer
    {
        // Reference to the Unity Monobehavoir class
        private readonly Server server;
        public TCPServer(Server server)
        {
            this.server = server;
        }
        
        private TcpListener tcpListener;

        // Start activates the TCP Server
        public void Start()
        {
            if (server.networkFeatureState.value != (int) FeatureState.starting) { return; }
            
            tcpListener = new TcpListener(IPAddress.Any, server.serverTCPPort.value);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
        }
        
        // TcpConnectCallback is called by TcpListener every time a client connects
        private void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            
            Debug.Log($"SERVER: Incoming connection from {tcpClient.Client.RemoteEndPoint}...");

            // Find a free Id for new client.
            for (byte i = 1; i <= State.MaxClients; i++)
            {
                if (UserController.instance.users.ContainsKey(i)) continue;
                
                // Needs to run on main Thread because Unity doesn't really support pure multi threading
                Threader.RunOnMainThread(() =>
                {
                    UserController.instance.UserJoined(i);
                    
                    User user = UserController.instance.users[i];
                    user.socket = tcpClient;
                    
                    ConnectClient(user);
                });
                return;
            }

            Debug.Log($"SERVER: {tcpClient.Client.RemoteEndPoint} failed to connect: Server full!");
        }

        // ConnectClient initializes a client. 
        public void ConnectClient(User user)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online) { return; }
            Debug.Log($"SERVER: Connecting Client " + user.id + "...");
            
            // Saving client Ip and Port data
            user.socket.ReceiveBufferSize = State.BufferSize;
            user.socket.SendBufferSize = State.BufferSize;
            user.stream = user.socket.GetStream();
            user.ip =  user.socket.Client.RemoteEndPoint.ToString().Split(':')[0];

            user.receiveBuffer = new byte[State.BufferSize];
            user.stream.BeginRead(user.receiveBuffer, 0, State.BufferSize, ReceiveCallback, user);
            
            // Notifying all already connected clients about new client
            foreach (byte id in UserController.instance.users.Keys)
            {
                if (id == user.id) {continue;}
                server.serverSend.ServerUserJoined(id, user.id);
            }
            
            // Sending new Client its id.
            server.serverSend.ServerInit(user.id);
        }
        
        // ReceiveCallback is called when the TCP Server receives data.
        private void ReceiveCallback(IAsyncResult result)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online && server.networkFeatureState.value != (int) FeatureState.starting) { return; }

            // Getting the user how send the data
            User user = (User) result.AsyncState;
            try
            {
                // Checking if data was actually send
                int byteLength = user.stream.EndRead(result);
                if (byteLength < State.HeaderSize)
                {
                    // In this case the user most likely disconnected or has major connection issues.
                    Threader.RunOnMainThread(() =>
                    {
                        server.DisconnectClient(user.id);
                    });
                    return;
                }
                
                // Copying data into different buffer so the receiving buffer can be cleared.
                byte[] data = new byte[byteLength];
                Array.Copy(user.receiveBuffer, data, byteLength);

                // Checking if the data should be handel async.
                if (BitConverter.ToBoolean(data, 0))
                {
                    server.HandelData(data);
                }
                else
                {
                    Threader.RunOnMainThread(() =>
                    {
                        server.HandelData(data);
                    });
                }
                
                // Making ready to receive new message
                user.stream.BeginRead(user.receiveBuffer, 0, State.BufferSize, ReceiveCallback, user);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Threader.RunOnMainThread(() =>
                {
                    server.DisconnectClient(user.id);
                });
            }
        }
        
        // SendData send the data to userId
        public void SendData(byte userId, byte[] data, int length)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online && server.networkFeatureState.value != (int) FeatureState.starting) { return; }

            try
            {
                UserController.instance.users[userId].stream.BeginWrite(data, 0, length, null, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to player {userId} via TCP: {ex}");
            }
        }

        // DisconnectClient disconnects the given client 
        public void DisconnectClient(byte userId)
        {
            User user = UserController.instance.users[userId];
            if (user.socket != null)
            {
                user.socket.Close();
            }
            user.stream = null;
            user.receiveBuffer = null;
            user.socket = null;
        }

        public void Stop()
        {
            tcpListener.Stop();
        }
    }
}