using System.Linq;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client.Code
{
    public class ClientHandle
    {
        private readonly Client client;
        public ClientHandle(Client client)
        {
            this.client = client;
        }
        
        public void DebugMessage(Packet packet)
        {
            string message = packet.ReadString();
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }
        
        public void ServerSettings(Packet packet)
        {
            client.clientId.value = packet.ReadByte();
            
            string version = packet.ReadString();
            string[] versions = version.Split(',');
            
            if (versions.Contains("1.0"))
            {
                client.serverUDPSupport = packet.ReadBool();
            }
            
            Debug.Log("CLIENT: received server settings:" +
                      "\nVersion " + version +
                      "\nUDP " + client.serverUDPSupport
            );
            
            client.clientSend.ClientSettings();
        }
        
        public void ServerStartUDP(Packet packet)
        {
            client.udpClient.Connect();
            client.clientSend.ClientUDPConnection();
        }
        
        public void ServerUDPConnection(Packet packet)
        {
            client.clientSend.ClientUDPConnectionStatus();
        }
        
        public void UserStatus(Packet packet)
        {
            byte user = packet.ReadByte();
            byte status = packet.ReadByte();
            
            if (status == 1)
            {
                client.userJoined.Raise(user);
            }
            else if (status == 2)
            {
                client.userLeft.Raise(user);
            }
            
            Debug.LogFormat("CLIENT: User: {0} Status: {1}", user, status);
        }
    }
}