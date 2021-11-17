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
        private readonly NetworkController network;

        public NetworkHandle(NetworkController network)
        {
            this.network = network;
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.featureSettings, FeatureSettings },
                { (byte)Packets.userVoiceId, UserVoiceID },
                { (byte)Packets.userPos, UserPos },
            };
        }
        
        public delegate void PacketHandler(byte userID, Packet packet);
        public Dictionary<byte, PacketHandler> packetHandlers;
        
        public void FeatureSettings(byte userID, Packet packet)
        {
            String log = "NETWORK: Received Feature Settings from "+ userID +":\n";
            
            User user = UserController.instance.users[userID];

            bool needToReply = packet.ReadBool();
            
            int lenght = packet.ReadInt32();
            for (int i = 0; i < lenght; i++)
            {
                String name = packet.ReadString();
                bool value = packet.ReadBool();
                
                log += name + " " + value + "\n";
                user.features[name] = value;
            }
            
            if (needToReply)
            {
                network.networkSend.FeatureSettings(userID, false);
            }
            
            if (network.networkFeatureState.value != (int) FeatureState.online)
            {
                bool allLoaded = true;
                foreach (User otherUser in UserController.instance.users.Values)
                {
                    if (!otherUser.features.ContainsKey("Network"))
                    {
                        allLoaded = false;
                    }
                }

                if (allLoaded)
                {
                    Debug.Log("Network: online");
                    network.networkFeatureState.value = (int) FeatureState.online;
                }
            }
        }
        
        public void UserVoiceID(byte userID, Packet packet)
        {
            byte vID = packet.ReadByte();
            bool reply = packet.ReadBool();

            User user = UserController.instance.users[userID];
            if (user == null)
            {
                Debug.Log("NETWORK: User not existing");
                return;
            }
            
            user.voiceId = vID;
            
            if (reply)
            {
                network.networkSend.UserVoiceID(false);
            }
            
            Debug.LogFormat("NETWORK: User: {0} VoiceID: {1}", userID, vID);
        }

        public void UserPos(byte userID, Packet packet)
        {
            float3 pos = packet.ReadFloat3();
            UserController.instance.users[userID].transform.position = pos;
        }
    }
}