using System.Threading;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Network.Server.Code
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
        
        public void ServerSettings(ServerClient client)
        {
            string version = "1.0";
            Debug.Log("SERVER: [" + client.id + "] sending settings" +
                  "\nVersion " + version +
                  "\nUDP " + server.featureSettings.UPDSupport
            );

            using Packet packet = new Packet((byte) Packets.serverSettings, 0);
            packet.Write(client.id);
                
            packet.Write(version);
            packet.Write(server.featureSettings.UPDSupport);

            server.Send(client, packet, false);
        }
        
        public void ServerStartUDP(ServerClient client)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] starting udp test");
            using (Packet packet = new Packet((byte) Packets.serverStartUDP, 0))
            {
                server.Send(client, packet, false);
            }
            
            Threader.RunAsync(() =>
            {
                Thread.Sleep(2000);
                if (client.updConnected) {return;}
                ServerUDPConnection(client, false);
            });
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