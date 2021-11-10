using System;
using System.Net;
using System.Net.Sockets;
using Network.Both;
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
        
            udpListener = new UdpClient(server.port.value);
            udpListener.BeginReceive(UdpReceiveCallback, null);
            
            server.udpFeatureState.value = (int) FeatureState.online;
        }
        
        private void UdpReceiveCallback(IAsyncResult result)
        {
            if (server.serverState != NetworkState.connected) { return; }
            
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if (data.Length < 6)
                {
                    Threader.RunOnMainThread(() =>
                    {
                        Debug.Log("SERVER no correct UDP packet size");
                    });
                    return;
                }

                foreach (ServerClient serverClient in server.clients)
                {
                    if (serverClient == null || serverClient.ip != clientEndPoint.Address.ToString()) continue;

                    if (!UserController.instance.users[serverClient.id].features["UDP"])
                    {
                        serverClient.endPoint = clientEndPoint;
                    }
                }

                server.HandelData(data);
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error receiving UDP data: {ex}");
            }
        }

        public void SendData(ServerClient client, byte[] data, int length)
        {
            if (server.serverState != NetworkState.connected) { return; }
            
            try
            {
                if (client.endPoint != null)
                {
                    udpListener.BeginSend(data, length, client.endPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"SERVER: Error sending data to {client.endPoint} via UDP: {ex}");
            }
        }

        public void DisconnectClient(ServerClient client)
        {
            client.endPoint = null;
        }

        public void Stop()
        {
            server.udpFeatureState.value = (int) FeatureState.stopping;
            
            udpListener.Close();
            
            server.udpFeatureState.value = (int) FeatureState.offline;
        }
    }
}