using System.Linq;
using Network.Both;
using UnityEngine;
using Utility;

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

                client.networkSend.FeatureSettings(userId, true);
            }
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
            bool udpOnline = packet.ReadBool();
            UserController.instance.users[0].features["UDP"] = udpOnline;
            client.clientSend.ClientUDPConnection();

            if (udpOnline)
            {
                client.udpFeatureState.value = (int) FeatureState.online;
            }
            else
            {
                client.udpFeatureState.value = (int) FeatureState.failed;
            }
        }
    }
}