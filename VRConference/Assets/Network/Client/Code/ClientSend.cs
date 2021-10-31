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
            client.Send(packet, false);
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

            client.Send(packet, false);
        }
        public void ClientUDPConnection()
        {
            Debug.Log("CLIENT: udp test message");
            using (Packet packet = new Packet((byte) Packets.clientUDPConnection, client.clientId.value))
            {
                client.Send(packet, true);
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
            client.Send(packet, false);
        }

        private void SendContainer(Packet containerPacket, Packet packet, bool useUDP)
        {
            containerPacket.Write(packet.ToArray());
            client.Send(containerPacket, useUDP);
        }
        public void ContainerToAll(Packet packet, bool useUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.all);
            containerPacket.Write(useUDP);

            SendContainer(containerPacket, packet, useUDP);
        }
        
        public void ContainerToAllExceptOrigin(Packet packet, bool useUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptOrigin);
            containerPacket.Write(useUDP);
            
            SendContainer(containerPacket, packet, useUDP);
        }
        
        public void ContainerToList(Packet packet, byte[] userIDs, bool useUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.list);
            containerPacket.Write(useUDP);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, useUDP);
        }
        
        public void ContainerToAllExceptList(Packet packet, byte[] userIDs, bool useUDP)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptList);
            containerPacket.Write(useUDP);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, useUDP);
        }
    }
}