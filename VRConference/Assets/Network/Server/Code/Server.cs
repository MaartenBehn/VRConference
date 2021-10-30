using System.Collections.Generic;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Server.Code
{
    public class Server : MonoBehaviour
    {
        public PublicInt port;
        public PublicInt serverState;
        
        private delegate void PacketHandler(byte userID, Packet packet);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        public TCPServer tcpServer;
        public UDPServer udpServer;
        public ServerHandle serverHandle;
        public ServerSend serverSend;
        
        public ServerClient[] clients;
        
        [SerializeField] private PublicEvent startServerEvent;
        [SerializeField] private PublicEvent stopServerEvent;
        [SerializeField] private PublicEventString debugMessageEvent;
        
        public NetworkFeatureSettings featureSettings;
        public PublicInt networkFeatureState;

        public PublicEventByte userJoined;
        public PublicEventByte userLeft;
        
        public PublicByte voiceID;
        
        private void Awake()
        {
            tcpServer = new TCPServer(this);
            udpServer = new UDPServer(this);
            serverHandle = new ServerHandle(this);
            serverSend = new ServerSend(this);
            clients = new ServerClient[State.MaxClients + 1];
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.debugMessage, serverHandle.DebugMessage },
                
                { (byte)Packets.clientSettings, serverHandle.ClientSettings },
                { (byte)Packets.clientUDPConnection, serverHandle.ClientUDPConnection },
                { (byte)Packets.clientUDPConnectionStatus, serverHandle.ClientUDPConnectionStatus },
                
                { (byte)Packets.clientSendToAllClients, serverHandle.ClientSendToAllClients },
                { (byte)Packets.userStatus, serverHandle.UserStatus },
                { (byte)Packets.userVoiceId, serverHandle.UserVoiceID },
            };

            startServerEvent.Register(StartServer);
            stopServerEvent.Register(StopServer);
            debugMessageEvent.Register(serverSend.DebugMessage);

            serverState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }

        private void OnApplicationQuit()
        {
            StopServer();
        }

        private void StartServer()
        {
            if (serverState.value != (int) NetworkState.notConnected) { return; }
            
            Debug.Log("SERVER: Starting...");
            serverState.value = (int) NetworkState.connecting;
            networkFeatureState.value = (int) FeatureState.starting;
            
            tcpServer.Start();
            if (featureSettings.UPDSupport)
            {
                udpServer.Start();
            }

            Debug.Log("SERVER: Started");
            serverState.value = (int) NetworkState.connected;
            networkFeatureState.value = (int) FeatureState.online;
        }

        public void ConnectClient(ServerClient client)
        {
            Debug.Log($"SERVER: Connecting Client " + client.id + "...");
            client.state = NetworkState.connecting;
            tcpServer.ConnectClient(client);
        }

        public void HandelData(byte[] data)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            int length = packet.ReadInt32();
            if (length + 4 != data.Length)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("SERVER: Packet size not correct.");
                });
                return;
            }
            
            byte packetId = packet.ReadByte();
            byte userID = packet.ReadByte();
            
            Threader.RunOnMainThread(() =>
            {
                packetHandlers[packetId](userID, packet);
            });
        }

        public void SendTCPData(ServerClient client, Packet packet)
        {
            packet.PrepareForSend();
            tcpServer.SendData(client, packet.ToArray(), packet.Length());
        }
        public void SendTCPDataToAll(Packet packet)
        {
            packet.PrepareForSend();
            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    tcpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void SendTCPDataToAll(Packet packet, ServerClient eClient)
        {
            packet.PrepareForSend();
            foreach (ServerClient client in clients)
            {
                if (client != null && client != eClient)
                {
                    tcpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void SendUDPData(ServerClient client, Packet packet)
        {
            if (!featureSettings.UPDSupport || !client.clientUdpSupport)
            {
                SendTCPData(client, packet);
                return;
            }
            
            packet.PrepareForSend();
            udpServer.SendData(client, packet.ToArray(), packet.Length());
        }
        public void SendUDPDataToAll(Packet packet)
        {
            packet.PrepareForSend();
            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    if (!featureSettings.UPDSupport || !client.clientUdpSupport)
                    {
                        SendTCPData(client, packet);
                        continue;
                    }
                    
                    udpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void SendUDPDataToAll(Packet packet, ServerClient eClient)
        {
            packet.PrepareForSend();
            foreach (ServerClient client in clients)
            {
                if (client != null && client != eClient)
                {
                    if (!featureSettings.UPDSupport || !client.clientUdpSupport)
                    {
                        SendTCPData(client, packet);
                        continue;
                    }
                    
                    udpServer.SendData(client, packet.ToArray(), packet.Length());
                }
            }
        }
        
        public void DisconnectClient(ServerClient client)
        {
            client.state = NetworkState.disconnecting;
            
            tcpServer.DisconnectClient(client);
            udpServer.DisconnectClient(client);
            clients[client.id] = null;
            
            Debug.Log("SERVER: Client " + client.id + " disconnected.");
        }

        private void StopServer()
        {
            if (serverState.value != (int) NetworkState.connected) { return; }
            
            Debug.Log("SERVER: Stopping...");
            serverState.value = (int) NetworkState.disconnecting;
            networkFeatureState.value = (int) FeatureState.stopping;

            foreach (ServerClient client in clients)
            {
                if (client != null)
                {
                    DisconnectClient(client);
                }
            }
            
            tcpServer.Stop();
            udpServer.Stop();

            Debug.Log("SERVER: Stopped");
            serverState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }
    }
}