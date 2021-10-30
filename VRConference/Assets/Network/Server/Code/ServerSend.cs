using System.Threading;
using Network.Both;
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
            server.SendTCPDataToAll(packet);
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

            server.SendTCPData(client, packet);
        }
        
        public void ServerStartUDP(ServerClient client)
        {
            Debug.LogFormat("SERVER: [" +client.id+ "] starting udp test");
            using (Packet packet = new Packet((byte) Packets.serverStartUDP, 0))
            {
                server.SendTCPData(client, packet);
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

            if (received)
            {
                server.SendUDPData(client, packet);
            }
            else
            {
                server.SendTCPData(client, packet);
            }
        }
        
        public void UserStatus(byte user, byte status)
        {
            using Packet packet = new Packet((byte) Packets.userStatus, user);
            packet.Write(status);
            server.SendTCPDataToAll(packet);
        }

        public void UserVoiceID(byte user, byte voiceID)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, user);
            packet.Write(voiceID);
            server.SendTCPDataToAll(packet);
        }
    }
}