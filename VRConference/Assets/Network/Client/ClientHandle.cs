using System.Collections.Generic;
using Engine;
using Network.Both;
using UnityEngine;
using User;
using Users;

namespace Network.Client
{
    public class ClientHandle
    {
        private readonly Client client;
        public ClientHandle(Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(byte userID, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("CLIENT: Debug: "+ message);
        }

        public void ServerInit(byte userID, Packet packet)
        {
            Debug.Log("CLIENT: Server init");
            client.clientId.value = packet.ReadByte();
            
            int clientLength = packet.ReadInt32();
            for (int i = 0; i < clientLength; i++)
            {
                byte userId = packet.ReadByte();
                UserController.instance.UserJoined(userId);
            }
            client.networkFeatureState.value = (int) FeatureState.online;
            
            client.networkSend.FeatureSettingsToAllExceptOrigen(true);
        }
        
        public void ServerUserJoined(byte userID, Packet packet)
        {
            byte id = packet.ReadByte();
            
            Debug.Log("CLIENT: User " +id+ " joined.");
            
            UserController.instance.UserJoined(id);
        }
        
        public void ServerUserLeft(byte userID, Packet packet)
        {
            byte id = packet.ReadByte();
            
            Debug.Log("CLIENT: User " +id+ " left.");
            
            UserController.instance.UserLeft(id);
        }

        public void ServerUDPConnection(byte userID, Packet packet)
        {
            UserController.instance.users[0].features["UDP"] = true;
            client.udpFeatureState.value = (int) FeatureState.online;
            client.clientSend.ClientUDPConnection();
        }
    }
}