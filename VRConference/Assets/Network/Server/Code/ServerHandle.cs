using Network.Both;
using UnityEngine;

namespace Network.Server.Code
{
    public class ServerHandle
    {
        private readonly Server server;
        public ServerHandle(Server server)
        {
            this.server = server;
        }
        
        public void DebugMessage(ServerClient fromClient, Packet packet)
        {
            string message = packet.ReadString();
            Debug.Log("SERVER: [" +fromClient.id+ "] Debug: " + message);
        }
        
        public void ClientSettings(ServerClient fromClient, Packet packet)
        {
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
        
        public void ClientUDPConnection(ServerClient fromClient, Packet packet)
        {
            fromClient.updConnected = true;
            server.serverSend.ServerUDPConnection(fromClient, true);
        }
        
        public void ClientUDPConnectionStatus(ServerClient fromClient, Packet packet)
        {
            fromClient.updConnected = packet.ReadBool() && server.featureSettings.UPDSupport;
            Debug.Log("SERVER: [" +fromClient.id+ "] UDP connection status: "+ fromClient.updConnected);
            fromClient.state = NetworkState.connected;
        }
    }
}