using System;
using System.Net;
using System.Net.Sockets;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
{
    public class UDPClient
    {
        private readonly Client client;

        public UDPClient(Client client)
        {
            this.client = client;
        }

        private UdpClient socket;
        private IPEndPoint endPoint;

         public void Connect()
         {
             client.udpFeatureState.value = (int) FeatureState.starting;

             try
             {
                 socket = new UdpClient(client.clientUDPPort.value);
             }
             catch (Exception e)
             {
                 Debug.Log(e);
                 client.udpFeatureState.value = (int) FeatureState.failed;
                 return;
             }
             
             endPoint = new IPEndPoint(IPAddress.Parse(client.ip.value), client.serverUDPPort.value);

             socket.Connect(endPoint);
             socket.BeginReceive(ReceiveCallback, null);
             
             Debug.Log($"CLIENT: starting UDP...");
             
             client.clientSend.ClientStartUDP();
         }
         
         private void ReceiveCallback(IAsyncResult result)
         {
             if (client.udpFeatureState.value != (int) FeatureState.online && client.udpFeatureState.value != (int) FeatureState.starting) {return;}
             
             try
             {
                 byte[] data = socket.EndReceive(result, ref endPoint);
                 socket.BeginReceive(ReceiveCallback, null);

                 if (data.Length < State.HeaderSize)
                 {
                     Debug.Log("Lost Connection");
                     Threader.RunOnMainThread(client.Disconnect);
                     return;
                 }

                 if (BitConverter.ToBoolean(data, 0))
                 {
                     client.HandleData(data);
                 }
                 else
                 {
                     Threader.RunOnMainThread(() =>
                     {
                         client.HandleData(data);
                     });
                 }

             }
             catch (Exception e)
             {
                 Debug.Log(e);
                 Threader.RunOnMainThread(client.Disconnect);
             }
         }
         
         public void SendData(byte[] data, int length)
         {
             if (client.udpFeatureState.value != (int) FeatureState.online && client.udpFeatureState.value != (int) FeatureState.starting) {return;}
             
             try
             {
                 if (socket != null)
                 {
                     socket.BeginSend(data, length, null, null);
                 }
             }
             catch (Exception ex)
             {
                 Debug.Log($"CLIENT: Error sending data to server via UDP: {ex}");
             }
         }
         
         public void Disconnect()
         {
             client.udpFeatureState.value = (int) FeatureState.stopping;
             
             if (socket != null)
             {
                 socket.Close();
             }
             endPoint = null;
             socket = null;
             
             client.udpFeatureState.value = (int) FeatureState.offline;
         }
    }
}