using System;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Network.Both
{
    public class NetworkSend : MonoBehaviour
    {
        [SerializeField] private Server.Code.Server server;
        [SerializeField] private Client.Code.Client client;
        [SerializeField] private PublicByte userID;
        [SerializeField] private PublicBool isServer;
        
        [SerializeField] private PublicByte voiceID;
        [SerializeField] private PublicEventFloat3 sendPosEvent;

        private void Awake()
        {
            sendPosEvent.Register(UserPos);
        }
        
        private void Send(Packet packet, byte userID, bool useUDP)
        {
            if (isServer.value)
            {
                server.Send(server.GetClient(userID), packet, useUDP);
            }
            else
            {
                client.Send(packet, useUDP);
            }
        }

        private void SendToAll(Packet packet, bool userUDP)
        {
            if (isServer.value)
            {
                server.serverSend.SendToAll(packet, userUDP);
            }
            else
            {
                client.clientSend.ContainerToAll(packet, userUDP);
            }
        }
        
        private void SendToAllExceptOrigen(Packet packet, bool userUDP)
        {
            if (isServer.value)
            {
                server.serverSend.SendToAllExceptOrigen(packet, userUDP);
            }
            else
            {
                client.clientSend.ContainerToAllExceptOrigin(packet, userUDP);
            }
        }
        
        private void SendToList(Packet packet, byte[] userIDs, bool userUDP)
        {
            if (isServer.value)
            {
                server.serverSend.SendToList(packet, userIDs, userUDP);
            }
            else
            {
                client.clientSend.ContainerToList(packet, userIDs,  userUDP);
            }
        }
        
        private void SendToAllExceptList(Packet packet, byte[] userIDs, bool userUDP)
        {
            if (isServer.value)
            {
                server.serverSend.SendToAllExceptList(packet, userIDs, userUDP);
            }
            else
            {
                client.clientSend.ContainerToAllExceptList(packet, userIDs, userUDP);
            }
        }
        
        public void UserStatus(byte status)
        {
            using Packet packet = new Packet((byte) Packets.userStatus, userID.value);
            packet.Write(status);
            SendToAllExceptOrigen(packet, false);
        }
        
        public void UserVoiceID()
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, userID.value);
            packet.Write(voiceID.value);
            SendToAllExceptOrigen(packet, false);
        }
        
        public void UserPos(float3 pos)
        {
            using Packet packet = new Packet((byte) Packets.userPos, userID.value);
            packet.Write(pos);
            SendToAllExceptOrigen(packet, true);
        }
    }
}