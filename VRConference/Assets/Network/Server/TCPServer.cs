using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Engine.User;
using Network.Both;
using UnityEngine;
using Utility;

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
            if (server.networkFeatureState.value != (int) FeatureState.starting) { return; }
            
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
                if (UserController.instance.users.ContainsKey(i)) continue;
                
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

        public void ConnectClient(User user)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online) { return; }
            Debug.Log($"SERVER: Connecting Client " + user.id + "...");
            
            user.socket.ReceiveBufferSize = State.BufferSize;
            user.socket.SendBufferSize = State.BufferSize;
            user.stream = user.socket.GetStream();
            user.ip =  user.socket.Client.RemoteEndPoint.ToString().Split(':')[0];

            user.receiveBuffer = new byte[State.BufferSize];
            user.stream.BeginRead(user.receiveBuffer, 0, State.BufferSize, ReceiveCallback, user);
            
            foreach (byte id in UserController.instance.users.Keys)
            {
                if (id == user.id) {continue;}
                server.serverSend.ServerUserJoined(id, user.id);
            }
            
            // Sending new Client its id.
            server.serverSend.ServerInit(user.id);
        }
        
        private void ReceiveCallback(IAsyncResult result)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online) { return; }

            User user = (User) result.AsyncState;
            try
            {
                int byteLength = user.stream.EndRead(result);
                if (byteLength < State.HeaderSize)
                {
                    server.DisconnectClient(user.id);
                    return;
                }

                byte[] data = new byte[byteLength];
                Array.Copy(user.receiveBuffer, data, byteLength);
                server.HandelData(data);
                
                user.stream.BeginRead(user.receiveBuffer, 0, State.BufferSize, ReceiveCallback, user);
            }
            catch
            {
                server.DisconnectClient(user.id);
            }
        }
        
        public void SendData(byte userId, byte[] data, int length)
        {
            if (server.networkFeatureState.value != (int) FeatureState.online) { return; }

            try
            {
                UserController.instance.users[userId].stream.BeginWrite(data, 0, length, null, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to player {userId} via TCP: {ex}");
            }
        }

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