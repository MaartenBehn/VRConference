using System;
using System.Net;
using System.Net.Sockets;
using Engine.User;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class UDPServer
    {
        private readonly Server server;
        public UDPServer(Server server)
        {
            this.server = server;
        }
        
        private UdpClient udpListener;
        
        public void Start()
        {
            server.udpFeatureState.value = (int) FeatureState.starting;
        
            udpListener = new UdpClient(server.serverUDPPort.value);
            udpListener.BeginReceive(UdpReceiveCallback, null);
            
            server.udpFeatureState.value = (int) FeatureState.online;
        }
        
        private void UdpReceiveCallback(IAsyncResult result)
        {
            if (server.udpFeatureState.value != (int) FeatureState.online && server.udpFeatureState.value != (int) FeatureState.starting) {return;}
            
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 6)
                {
                    Debug.Log("SERVER no correct UDP packet size");
                    return;
                }

                foreach (User user in UserController.instance.users.Values)
                {
                    if (user.ip != clientEndPoint.Address.ToString()) continue;

                    if (!user.HasFeature("UDP"))
                    {
                        user.endPoint = clientEndPoint;
                    }
                }
                
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
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error receiving UDP data: {ex}");
            }
        }

        public void SendData(byte userId, byte[] data, int length)
        {
            if (server.udpFeatureState.value != (int) FeatureState.online && server.udpFeatureState.value != (int) FeatureState.starting) {return;}
            
            User user = UserController.instance.users[userId];
            
            try
            {
                if (user.endPoint != null)
                {
                    udpListener.BeginSend(data, length, user.endPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to {user.endPoint} via UDP: {ex}");
            }
        }

        public void DisconnectClient(byte userId)
        {
            User user = UserController.instance.users[userId];
            user.endPoint = null;
        }

        public void Stop()
        {
            server.udpFeatureState.value = (int) FeatureState.stopping;
            
            udpListener.Close();
            
            server.udpFeatureState.value = (int) FeatureState.offline;
        }
    }
}