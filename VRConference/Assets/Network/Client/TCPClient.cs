using System;
using System.Net.Sockets;
using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
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
             Debug.Log("CLIENT: Connecting TCP...");
             
             socket = new TcpClient
            {
                ReceiveBufferSize = State.BufferSize,
                SendBufferSize = State.BufferSize
            };

            receiveBuffer = new byte[State.BufferSize];
            socket.BeginConnect(client.ip.value, client.serverPort.value, ConnectCallback, socket);
            
            client.networkFeatureState.value = (int) FeatureState.starting;

            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.networkFeatureState.value == (int) FeatureState.starting)
                {
                    Threader.RunOnMainThread(() =>
                    {
                        Debug.Log("CLIENT: Connect Timeout");
                    });
                    socket.Close();
                    client.networkFeatureState.value = (int) FeatureState.failed;
                }
            });
        }
         
         private void ConnectCallback(IAsyncResult result)
         {
             socket.EndConnect(result);

             if (!socket.Connected)
             {
                 client.networkFeatureState.value = (int) FeatureState.offline;
                 return;
             }

             Threader.RunOnMainThread(() =>
             {
                 Debug.Log("CLIENT: Connected TCP");
             });

             stream = socket.GetStream();
             
             stream.BeginRead(receiveBuffer, 0, State.BufferSize, ReceiveCallback, null);
         }
         
          private void ReceiveCallback(IAsyncResult result)
         {
             if (client.networkFeatureState.value != (int) FeatureState.online && client.networkFeatureState.value != (int) FeatureState.starting) { return; }
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
             if (client.networkFeatureState.value != (int) FeatureState.online && client.networkFeatureState.value != (int) FeatureState.starting) { return; }
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