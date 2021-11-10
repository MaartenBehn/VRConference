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
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }

        public void ServerClientId(byte userID, Packet packet)
        {
            client.clientId.value = packet.ReadByte();
            client.networkFeatureState.value = (int) FeatureState.online;
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