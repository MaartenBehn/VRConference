using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Server.Code
{
    public class ServerHandle
    {
        private readonly Server server;
        public ServerHandle(Server server)
        {
            this.server = server;
        }

        private ServerClient GetClient(byte userID)
        {
            if (userID != 0)
            {
                return server.clients[userID];
            }
            
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("SERVER: Cant get Client from Message with Server or wrong ID. What the fuck are you doing?");
            });

            return null;
        }
        
        public void DebugMessage(byte userID, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: [" +userID+ "] Debug: " + message);
        }
        
        public void ClientSettings(byte userID, Packet packet)
        {
            ServerClient fromClient = GetClient(userID);
            
            string version = packet.ReadString();

            if (version == "1.0")
            { 
                fromClient.clientUdpSupport = packet.ReadBool();
            }
            
            Debug.Log("SERVER: [" +fromClient.id+ "] received client settings:" +
                      "\nVersion " + version +
                      "\nUDP " + fromClient.clientUdpSupport
            );

            if (server.featureSettings.UPDSupport && fromClient.clientUdpSupport)
            {
                server.serverSend.ServerStartUDP(fromClient);
            }
            else
            {
                fromClient.state = NetworkState.connected;
            }
        }
        
        public void ClientUDPConnection(byte userID, Packet packet)
        {
            ServerClient fromClient = GetClient(userID);
            
            fromClient.updConnected = true;
            server.serverSend.ServerUDPConnection(fromClient, true);
        }
        
        public void ClientUDPConnectionStatus(byte userID, Packet packet)
        {
            ServerClient fromClient = GetClient(userID);
            
            fromClient.updConnected = packet.ReadBool() && server.featureSettings.UPDSupport;
            Debug.Log("SERVER: [" +fromClient.id+ "] UDP connection status: "+ fromClient.updConnected);
            fromClient.state = NetworkState.connected;
        }
        
        public void ClientSendToAllClients(byte userID, Packet containerPacket)
        {
            ServerClient fromClient = GetClient(userID);
            
            byte[] data = containerPacket.ReadBytes(containerPacket.Length() - State.HeaderSize);
            using Packet packet = new Packet(data);
            packet.PrepareForSend();
            
            server.SendTCPDataToAll(packet, fromClient);
            server.HandelData(packet.ToArray());
        }
        
        public void UserStatus(byte userID, Packet packet)
        {
            byte status = packet.ReadByte();

            if (status == 1)
            {
                server.serverSend.UserStatus(0,1);
                server.userJoined.Raise(userID);
            }
            else if (status == 2)
            {
                server.userLeft.Raise(userID);
            }
            
            Debug.LogFormat("CLIENT: User: {0} Status: {1}", userID, status);
        }
        
        public void UserVoiceID(byte userID, Packet packet)
        {
            byte voiceID = packet.ReadByte();

            UserController.instance.users[userID].voiceId = voiceID;
            
            server.serverSend.UserVoiceID(0, server.voiceID.value);
        }
    }
}