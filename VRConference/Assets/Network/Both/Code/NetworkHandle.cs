using System;
using System.Collections.Generic;
using Engine.User;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Network.Both
{
    public class NetworkHandle : MonoBehaviour
    {
        public delegate void PacketHandler(byte userID, Packet packet);
        public Dictionary<byte, PacketHandler> packetHandlers;
        
        [SerializeField] private NetworkSend networkSend;
        [SerializeField] private PublicBool isServer;
        
        public PublicEventByte userJoined;
        public PublicEventByte userLeft;
        
        private void Awake()
        {
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.userStatus, UserStatus },
                { (byte)Packets.userVoiceId, UserVoiceID },
                { (byte)Packets.userPos, UserPos },
            };
            
        }

        public void UserStatus(byte userID, Packet packet)
        {
            byte status = packet.ReadByte();
            
            if (status == 1)
            {
                if (isServer.value)
                {
                    networkSend.UserStatus(1);
                }
                userJoined.Raise(userID);
            }
            else if (status == 2)
            {
                userLeft.Raise(userID);
            }
            
            Debug.LogFormat("NETWORK: User: {0} Status: {1}", userID, status);
        }
        
        public void UserVoiceID(byte userID, Packet packet)
        {
            byte vID = packet.ReadByte();

            User user = UserController.instance.users[userID];
            if (user == null)
            {
                Debug.Log("NETWORK: User not existing");
                return;
            }

            user.SetVoiceId(vID);

            if (isServer.value)
            {
                networkSend.UserVoiceID();
            }
            
            Debug.LogFormat("NETWORK: User: {0} VoiceID: {1}", userID, vID);
        }

        public void UserPos(byte userID, Packet packet)
        {
            float3 pos = packet.ReadFloat3();
            UserController.instance.users[userID].transform.position = pos;
            
            Debug.LogFormat("NETWORK: User: {0} Pos: {1}", userID, pos);
        }
    }
}