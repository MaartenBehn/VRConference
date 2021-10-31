using System.Threading;
using Network.Both;
using Unity.Mathematics;
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

        private void SendContainer(Packet containerPacket, Packet packet, bool userUDP)
        {
            containerPacket.Write(packet.ToArray());
            if (!userUDP)
            {
                client.SendTCPData(containerPacket);
            }
            else
            {
                client.SendUDPData(containerPacket);
            }
        }
        public void ContainerToAll(Packet packet, bool userUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.all);
            containerPacket.Write(userUDP);

            SendContainer(containerPacket, packet, userUDP);
        }
        
        public void ContainerToAllExceptOrigin(Packet packet, bool userUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptOrigin);
            containerPacket.Write(userUDP);
            
            SendContainer(containerPacket, packet, userUDP);
        }
        
        public void ContainerToList(Packet packet, byte[] userIDs, bool userUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.list);
            containerPacket.Write(userUDP);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, userUDP);
        }
        
        public void ContainerToAllExceptList(Packet packet, byte[] userIDs, bool userUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptList);
            containerPacket.Write(userUDP);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, userUDP);
        }
        
        public void UserStatus(byte status)
        {
            using Packet packet = new Packet((byte) Packets.userStatus, client.clientId.value);
            packet.Write(status);
            ContainerToAllExceptOrigin(packet, false);
        }
        
        public void UserVoiceID(byte voiceID)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, client.clientId.value);
            packet.Write(voiceID);
            ContainerToAllExceptOrigin(packet, false);
        }
        
        public void UserPos(float3 pos)
        {
            using Packet packet = new Packet((byte) Packets.userPos, client.clientId.value);
            packet.Write(pos);
            ContainerToAllExceptOrigin(packet, true);
        }
    }
}