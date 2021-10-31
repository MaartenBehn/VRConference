using Engine.User;
using Network.Both;
using Unity.Mathematics;
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
        
        public void ClientContainerPacket(byte userID, Packet containerPacket)
        { 
            ServerClient fromClient = GetClient(userID);

            byte type = containerPacket.ReadByte();
            bool useUDP = containerPacket.ReadBool();

            byte[] userIDs = null;
            if (type == (byte) ContainerType.list || type == (byte) ContainerType.allExceptList)
            {
                int length = containerPacket.ReadInt32();
                userIDs = containerPacket.ReadBytes(length);
            }

            byte[] data = containerPacket.ReadBytes(containerPacket.UnReadLength());
            using Packet packet = new Packet(data);
            
            packet.PrepareForSend();
            
            if (type == (byte) ContainerType.all)
            {
                if (!useUDP)
                {
                    server.SendTCPDataToAll(packet);
                }
                else
                {
                    server.SendUDPDataToAll(packet);
                }
                
                server.HandelData(packet.ToArray());
            }
            else if (type == (byte) ContainerType.allExceptOrigin)
            {
                if (!useUDP)
                {
                    server.SendTCPDataToAll(packet, fromClient);
                }
                else
                {
                    server.SendUDPDataToAll(packet, fromClient);
                }
                server.HandelData(packet.ToArray());
            }
            else if (type == (byte) ContainerType.list)
            {
                for (int i = 0; i < userIDs.Length; i++)
                {
                    if (userIDs[i] == 0)
                    {
                        server.HandelData(packet.ToArray());
                    }
                    else
                    {
                        if (!useUDP)
                        {
                            server.SendTCPData(GetClient(userIDs[i]), packet);
                        }
                        else
                        {
                            server.SendUDPData(GetClient(userIDs[i]), packet);
                        }
                    }
                }
            }
            else if (type == (byte) ContainerType.allExceptList)
            {
                bool send;
                foreach (ServerClient client in server.clients)
                {
                    send = true;
                    for (int i = 0; i < userIDs.Length; i++)
                    {
                        if (client.id == userIDs[i])
                        {
                            send = false;
                            break;
                        }
                    }

                    if (send)
                    {
                        if (!useUDP)
                        {
                            server.SendTCPData(client, packet);
                        }
                        else
                        {
                            server.SendUDPData(client, packet);
                        }
                    }
                }
                
                send = true;
                for (int i = 0; i < userIDs.Length; i++)
                {
                    if (userIDs[i] == 0)
                    {
                        send = false;
                        break;
                    }
                }
                
                if (send)
                {
                    server.HandelData(packet.ToArray());
                }
            }
        }
        
        public void UserStatus(byte userID, Packet packet)
        {
            byte status = packet.ReadByte();

            if (status == 1)
            {
                server.serverSend.UserStatus(1);
                server.userJoined.Raise(userID);
            }
            else if (status == 2)
            {
                server.userLeft.Raise(userID);
            }
            
            Debug.LogFormat("SERVER: User: {0} Status: {1}", userID, status);
        }
        
        public void UserVoiceID(byte userID, Packet packet)
        {
            byte voiceID = packet.ReadByte();

            User user = UserController.instance.users[userID];
            if (user == null)
            {
                Debug.Log("SERVER: User not existing");
                return;
            }
            user.voiceId = voiceID;
            
            server.serverSend.UserVoiceID(server.voiceID.value);
            
            Debug.LogFormat("SERVER: User: {0} VoiceID: {1}", userID, voiceID);
        }
        
        public void UserPos(byte userID, Packet packet)
        {
            float3 pos = packet.ReadFloat3();
            UserController.instance.users[userID].transform.position = pos;
            
            Debug.LogFormat("SERVER: User: {0} Pos: {1}", userID, pos);
        }
    }
}