using System.Threading;
using Engine;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
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
            client.Send(packet, false, false);
        }
        
        public void ClientStartUDP()
        {
            Debug.Log("CLIENT: start udp test");
            using Packet packet = new Packet((byte) Packets.clientStartUDP, client.clientId.value);
            packet.PrepareForSend(false);
            client.udpClient.SendData(packet.ToArray(), packet.Length());

            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.udpFeatureState.value == (int) FeatureState.online) return;
                
                client.udpFeatureState.value = (int) FeatureState.failed;
                ClientUDPConnection();
            });
        }
        
        public void ClientUDPConnection()
        {
            bool udpOnline = client.udpFeatureState.value == (int) FeatureState.online;
            Debug.Log("CLIENT: UDP connection status: "+ udpOnline);
            
            using Packet packet = new Packet((byte) Packets.clientUDPConnection, client.clientId.value);
            packet.Write(udpOnline);
            client.Send(packet, false, false);
        }
        

        private void SendContainer(Packet containerPacket, Packet packet, bool useUDP, bool async)
        {
            containerPacket.Write(packet.ToArray());
            client.Send(containerPacket, useUDP, async);
        }
        public void ContainerToAll(Packet packet, bool useUDP, bool async)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.all);
            containerPacket.Write(useUDP);
            containerPacket.Write(async);

            SendContainer(containerPacket, packet, useUDP, async);
        }
        
        public void ContainerToAllExceptOrigin(Packet packet, bool useUDP, bool async)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptOrigin);
            containerPacket.Write(useUDP);
            containerPacket.Write(async);
            
            SendContainer(containerPacket, packet, useUDP, async);
        }
        
        public void ContainerToList(Packet packet, byte[] userIDs, bool useUDP, bool async)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.list);
            containerPacket.Write(useUDP);
            containerPacket.Write(async);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, useUDP, async);
        }
        
        public void ContainerToAllExceptList(Packet packet, byte[] userIDs, bool useUDP, bool async)
        {
            using Packet containerPacket = new Packet((byte) Packets.clientContainerPacket, client.clientId.value);
            packet.PrepareForRead();
            containerPacket.Write((byte) ContainerType.allExceptList);
            containerPacket.Write(useUDP);
            containerPacket.Write(async);
            containerPacket.Write(userIDs.Length);
            containerPacket.Write(userIDs);
            
            SendContainer(containerPacket, packet, useUDP, async);
        }
    }
}