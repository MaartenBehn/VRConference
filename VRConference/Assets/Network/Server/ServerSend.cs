using System.Threading;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class ServerSend
    {
        private readonly Server server;
        public ServerSend(Server server)
        {
            this.server = server;
        }
        
        public void DebugMessage(string message)
        {
            using Packet packet = new Packet((byte) Packets.debugMessage, 0);
            packet.Write(message);
            SendToAll(packet, false);
        }
        
        public void ServerClientId(ServerClient client)
        {
            using Packet packet = new Packet((byte) Packets.serverClientId, 0);
            packet.Write(client.id);
            
            server.Send(client, packet, false);
        }
        
        public void ServerUDPConnection(ServerClient client, bool received)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] udp test message");
            using Packet packet = new Packet((byte) Packets.serverUDPConnection, 0);
            packet.Write(received);

            server.Send(client, packet, received);
        }

        public void SendToAll(Packet packet, bool useUDP)
        {
            server.HandelData(packet.ToArray());
            SendToAllExceptOrigen(packet, useUDP);
        }
        
        public void SendToAllExceptOrigen(Packet packet, bool useUDP)
        {
            foreach (ServerClient client in server.clients)
            {
                if (client != null)
                {
                    server.Send(client, packet, useUDP);
                }
            }
        }
        
        public void SendToList(Packet packet, byte[] userIDs, bool useUDP)
        {
            for (int i = 0; i < userIDs.Length; i++)
            {
                server.Send(server.GetClient(userIDs[i]), packet, useUDP);
            }
        }

        public void SendToAllExceptList(Packet packet, byte[] userIDs, bool useUDP)
        {
            foreach (ServerClient client in server.clients)
            {
                if (client == null)
                {
                    continue;
                }
                
                bool send = true;
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
        }
    }
}