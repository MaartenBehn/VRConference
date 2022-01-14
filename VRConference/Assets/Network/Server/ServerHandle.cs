using Network.Both;
using UnityEngine;
using Users;

namespace Network.Server
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
        
        public void ClientStartUDP(byte userID, Packet packet)
        {
            server.serverSend.ServerUDPConnection(userID);
        }
        
        public void ClientUDPConnection(byte userID, Packet packet)
        {
            bool updOnline = packet.ReadBool();
            UserController.instance.users[userID].features["UDP"] = updOnline;
        }
        
        public void ClientContainerPacket(byte userID, Packet containerPacket)
        {
            byte type = containerPacket.ReadByte();
            bool useUDP = containerPacket.ReadBool();
            bool async = containerPacket.ReadBool();

            byte[] userIDs = null;
            if (type == (byte) ContainerType.list || type == (byte) ContainerType.allExceptList)
            {
                int length = containerPacket.ReadInt32();
                userIDs = containerPacket.ReadBytes(length);
            }

            byte[] data = containerPacket.ReadBytes(containerPacket.UnReadLength());
            using Packet packet = new Packet(data);
            packet.PrepareForSend(async);

            if (type == (byte) ContainerType.all)
            {
                server.serverSend.SendToAll(packet, useUDP, async);
                server.HandelData(packet.ToArray());
            }
            else if (type == (byte) ContainerType.allExceptOrigin)
            {
                server.serverSend.SendToAllExceptList(packet, new []{userID}, useUDP, async);
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
                        server.Send(userIDs[i], packet, useUDP, async);
                    }
                }
            }
            else if (type == (byte) ContainerType.allExceptList)
            {
                bool send;
                foreach (byte id in UserController.instance.users.Keys)
                {
                    send = true;
                    for (int i = 0; i < userIDs.Length; i++)
                    {
                        if (id == userIDs[i])
                        {
                            send = false;
                            break;
                        }
                    }

                    if (send)
                    {
                        server.Send(id, packet, useUDP, async);
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