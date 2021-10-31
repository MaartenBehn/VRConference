using System.Linq;
using Engine.User;
using Network.Both;
using Unity.Mathematics;
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
        
        public void DebugMessage(byte userID, Packet packet)
        {
            string message = packet.ReadString();
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("CLIENT: Debug: "+ message);
            });
        }
        
        public void ServerSettings(byte userID, Packet packet)
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
        
        public void ServerStartUDP(byte userID, Packet packet)
        {
            client.udpClient.Connect();
            client.clientSend.ClientUDPConnection();
        }
        
        public void ServerUDPConnection(byte userID, Packet packet)
        {
            client.clientSend.ClientUDPConnectionStatus();
        }
    }
}