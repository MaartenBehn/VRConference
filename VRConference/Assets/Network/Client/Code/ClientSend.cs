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
            using Packet packet = new Packet((byte) Packets.debugMessage, client.clientId.value);
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

            using Packet packet = new Packet((byte) Packets.clientSettings, client.clientId.value);
            packet.Write(version);
            packet.Write(client.featureSettings.UPDSupport);

            client.SendTCPData(packet);
        }
        public void ClientUDPConnection()
        {
            Debug.Log("CLIENT: udp test message");
            using (Packet packet = new Packet((byte) Packets.clientUDPConnection, client.clientId.value))
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
            using Packet packet = new Packet((byte) Packets.clientUDPConnectionStatus, client.clientId.value);
            packet.Write(client.udpOnline.value);
            client.SendTCPData(packet);
        }
        
        public void ClientSendToAllClients(Packet packet)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientSendToAllClients, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write(packet.ToArray());
            client.SendTCPData(containerPacket);
        }
        
        public void UserStatus(byte status)
        {
            using Packet packet = new Packet((byte) Packets.userStatus, client.clientId.value);
            packet.Write(status);
            ClientSendToAllClients(packet);
        }
        
        public void UserVoiceID(byte voiceID)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, client.clientId.value);
            packet.Write(voiceID);
            ClientSendToAllClients(packet);
        }
    }
}