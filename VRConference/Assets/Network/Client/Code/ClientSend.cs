using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client.Code
{
    public class ClientSend
    {
        private readonly Client client;
        public ClientSend(Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(string message)
        {
            using Packet packet = new Packet((byte) Packets.debugMessage);
            packet.Write(message);
            client.SendTCPData(packet);
        }
        
        public void ClientSettings()
        {
            string version = "1.0";
            Debug.Log("CLIENT: sending settings: " +
                  "\nVersion " + version +
                  "\nUDP " + client.featureSettings.UPDSupport
            );

            using Packet packet = new Packet((byte) Packets.clientSettings);
            packet.Write(version);
            packet.Write(client.featureSettings.UPDSupport);

            client.SendTCPData(packet);
        }
        public void ClientUDPConnection()
        {
            Debug.Log("CLIENT: udp test message");
            using (Packet packet = new Packet((byte) Packets.clientUDPConnection))
            {
                client.SendUDPData(packet);
            }
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.udpOnline.value) return;
                
                client.serverUDPSupport = false;
                ClientUDPConnectionStatus();
            });
        }
        public void ClientUDPConnectionStatus()
        {
            client.udpOnline.value = client.featureSettings.UPDSupport && client.serverUDPSupport;
            Debug.Log("CLIENT: UDP connection status: "+ client.udpOnline.value);
            using Packet packet = new Packet((byte) Packets.clientUDPConnectionStatus);
            packet.Write(client.udpOnline.value);
            client.SendTCPData(packet);
        }
    }
}