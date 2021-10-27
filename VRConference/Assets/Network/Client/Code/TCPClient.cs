using System;
using System.Net.Sockets;
using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client.Code
{
    public class TCPClient
    {
        private readonly Client client;

        public TCPClient(Client client)
        {
            this.client = client;
        }

        private TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;
        
         public void Connect()
         {
             if (client.clientState.value != (int) NetworkState.notConnected) { return; }
             
             Debug.Log("CLIENT: Connecting TCP...");
             
             socket = new TcpClient
            {
                ReceiveBufferSize = State.BufferSize,
                SendBufferSize = State.BufferSize
            };

            receiveBuffer = new byte[State.BufferSize];
            socket.BeginConnect(client.ip.value, client.serverPort.value, ConnectCallback, socket);
            client.clientState.value = (int) NetworkState.connecting;
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.clientState.value == (int) NetworkState.connecting)
                {
                    Threader.RunOnMainThread(() =>
                    {
                        Debug.Log("CLIENT: Connect Timeout");
                    });
                    socket.Close();
                    client.clientState.value = (int) NetworkState.notConnected;
                }
            });
        }
         
         private void ConnectCallback(IAsyncResult result)
         {
             socket.EndConnect(result);

             if (!socket.Connected)
             {
                 client.clientState.value = (int) NetworkState.notConnected;
                 return;
             }

             Threader.RunOnMainThread(() =>
             {
                 Debug.Log("CLIENT: Connected TCP");
             });
             client.clientState.value = (int) NetworkState.connected;

             stream = socket.GetStream();
             
             stream.BeginRead(receiveBuffer, 0, State.BufferSize, ReceiveCallback, null);
         }
         
          private void ReceiveCallback(IAsyncResult result)
         {
             if (client.clientState.value != (int) NetworkState.connected) { return; }
             
             try
             {
                 int byteLength = stream.EndRead(result);
                 if (byteLength < State.HeaderSize)
                 {
                     client.Disconnect();
                     return;
                 }

                 byte[] data = new byte[byteLength];
                 Array.Copy(receiveBuffer, data, byteLength);

                 client.HandleData(data);
                 stream.BeginRead(receiveBuffer, 0, State.BufferSize, ReceiveCallback, null);
             }
             catch
             {
                 client.Disconnect();
             }
         }
         
         public void SendData(byte[] data, int length)
         {
             if (client.clientState.value != (int) NetworkState.connected) { return; }
             try
             {
                 if (socket != null)
                 {
                     stream.BeginWrite(data, 0, length, null, null);
                 }
             }
             catch (Exception ex)
             {
                 Debug.Log($"CLIENT:: Error sending data to server via TCP: {ex}");
             }
         }
         
         public void Disconnect()
         {
             if (socket != null)
             {
                 socket.Close();
             }
            
             stream = null;
             receiveBuffer = null;
             socket = null;
         }
    }
}