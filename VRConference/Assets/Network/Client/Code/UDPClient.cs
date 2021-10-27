using System;
using System.Net;
using System.Net.Sockets;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client.Code
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
             socket = new UdpClient(client.clientPort.value);
             endPoint = new IPEndPoint(IPAddress.Parse(client.ip.value), client.serverPort.value);

             socket.Connect(endPoint);
             socket.BeginReceive(ReceiveCallback, null);
             
             Debug.Log($"CLIENT: starting UDP...");
         }
         
         private void ReceiveCallback(IAsyncResult result)
         {
             if (client.clientState.value != (int) NetworkState.connected) { return; }
                
             try
             {
                 byte[] data = socket.EndReceive(result, ref endPoint);
                 socket.BeginReceive(ReceiveCallback, null);

                 if (data.Length < State.HeaderSize)
                 {
                     client.Disconnect();
                     return;
                 }
                 client.HandleData(data);
             }
             catch (Exception e)
             {
                 Threader.RunOnMainThread(() =>
                 {
                     Debug.Log(e);
                 });
                 //client.Disconnect();
             }
         }
         
         public void SendData(byte[] data, int length)
         {
             if (client.clientState.value != (int) NetworkState.connected) { return; }
             
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
             if (socket != null)
             {
                 socket.Close();
             }
             endPoint = null;
             socket = null;
         }
    }
}