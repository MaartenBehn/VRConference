using System;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;

namespace Network
{
    public class NetworkSend
    {
        private readonly NetworkController network;

        public NetworkSend(NetworkController network)
        {
            this.network = network;
        }

        private void Send(Packet packet, byte userID, bool useUDP)
        {
            if (network.isServer.value)
            {
                network.server.Send(userID, packet, useUDP);
            }
            else
            {
                SendToList(packet, new[] {userID}, useUDP);
            }
        }

        private void SendToAll(Packet packet, bool userUDP)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAll(packet, userUDP);
            }
            else
            {
                network.client.clientSend.ContainerToAll(packet, userUDP);
            }
        }
        
        private void SendToAllExceptOrigen(Packet packet, bool userUDP)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAllExceptOrigen(packet, userUDP);
            }
            else
            {
                network.client.clientSend.ContainerToAllExceptOrigin(packet, userUDP);
            }
        }
        
        private void SendToList(Packet packet, byte[] userIDs, bool userUDP)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToList(packet, userIDs, userUDP);
            }
            else
            {
                network.client.clientSend.ContainerToList(packet, userIDs,  userUDP);
            }
        }
        
        private void SendToAllExceptList(Packet packet, byte[] userIDs, bool userUDP)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAllExceptList(packet, userIDs, userUDP);
            }
            else
            {
                network.client.clientSend.ContainerToAllExceptList(packet, userIDs, userUDP);
            }
        }
        
        public void FeatureSettings(byte toUser, bool getResponse)
        {
            String log = "NETWORK: Sending Feature Settings :\n";
            log += "User " + toUser + "\n";
            
            using Packet packet = new Packet((byte) Packets.featureSettings, network.userId.value);
            packet.Write(getResponse);

            packet.Write(network.featureSettings.features.Length);
            foreach (var feature in network.featureSettings.features)
            {
                log += feature.name + " " + feature.active + "\n";
                packet.Write(feature.name);
                packet.Write(feature.active);
            }
            
            Send(packet, toUser, false);
            Debug.Log(log);
        }
        
        public void UserVoiceID(bool wantRelpy)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, network.userId.value);
            packet.Write(network.voiceId.value);
            packet.Write(wantRelpy);
            SendToAllExceptOrigen(packet, false);
        }
        
        public void UserPos(float3 pos)
        {
            using Packet packet = new Packet((byte) Packets.userPos, network.userId.value);
            packet.Write(pos);
            SendToAllExceptOrigen(packet, true);
        }
    }
}