using System;
using System.Collections.Generic;
using Engine.User;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;

namespace Network
{
    public class NetworkHandle
    {
        private NetworkController network;

        public NetworkHandle(NetworkController network)
        {
            this.network = network;
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.userStatus, UserStatus },
                { (byte)Packets.featureSettings, FeatureSettings },
                { (byte)Packets.userVoiceId, UserVoiceID },
                { (byte)Packets.userPos, UserPos },
            };
        }
        
        public delegate void PacketHandler(byte userID, Packet packet);
        public Dictionary<byte, PacketHandler> packetHandlers;

        public void UserStatus(byte userID, Packet packet)
        {
            byte status = packet.ReadByte();
            
            if (status == 1)
            {
                if (network.isServer.value)
                {
                    network.networkSend.UserStatus(1);
                }
                network.userJoined.Raise(userID);
            }
            else if (status == 2)
            {
                network.userLeft.Raise(userID);
            }
            
            Debug.LogFormat("NETWORK: User: {0} Status: {1}", userID, status);
        }
        
        public void FeatureSettings(byte userID, Packet packet)
        {
            String log = "Received Feature Settings from "+ userID +":\n";
            
            User user = UserController.instance.users[userID];

            int lenght = packet.ReadInt32();
            for (int i = 0; i < lenght; i++)
            {
                String name = packet.ReadString();
                bool value = packet.ReadBool();
                
                log += name + " " + value + "\n";
                user.features[name] = value;
            }
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

            user.voiceId = vID;

            if (network.isServer.value)
            {
                network.networkSend.UserVoiceID();
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