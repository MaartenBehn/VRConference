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
        
        public void DebugMessage(byte userID, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: [" +userID+ "] Debug: " + message);
        }
        
        public void ClientSettings(byte userID, Packet packet)
        {
            ServerClient fromClient = server.GetClient(userID);
            
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
            ServerClient fromClient = server.GetClient(userID);
            
            fromClient.updConnected = true;
            server.serverSend.ServerUDPConnection(fromClient, true);
        }
        
        public void ClientUDPConnectionStatus(byte userID, Packet packet)
        {
            ServerClient fromClient = server.GetClient(userID);
            
            fromClient.updConnected = packet.ReadBool() && server.featureSettings.UPDSupport;
            Debug.Log("SERVER: [" +fromClient.id+ "] UDP connection status: "+ fromClient.updConnected);
            fromClient.state = NetworkState.connected;
        }
        
        public void ClientContainerPacket(byte userID, Packet containerPacket)
        {
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
                server.serverSend.SendToAll(packet, useUDP);
                server.HandelData(packet.ToArray());
            }
            else if (type == (byte) ContainerType.allExceptOrigin)
            {
                server.serverSend.SendToAllExceptList(packet, new []{userID}, useUDP);
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
                        server.Send(server.GetClient(userIDs[i]), packet, useUDP);
                    }
                }
            }
            else if (type == (byte) ContainerType.allExceptList)
            {
                bool send;
                foreach (ServerClient client in server.clients)
                {
                    if (client == null)
                    {
                        continue;
                    }
                    
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
                        server.Send(client, packet, useUDP);
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
        
        
    }
}