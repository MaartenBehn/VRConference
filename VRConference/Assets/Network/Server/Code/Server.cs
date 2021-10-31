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
        public PublicByte userId;
        
        private static Dictionary<byte, NetworkHandle.PacketHandler> packetHandlers;

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
        
        public NetworkSend networkSend;
        public NetworkHandle networkHandle;

        private void Awake()
        {
            tcpServer = new TCPServer(this);
            udpServer = new UDPServer(this);
            serverHandle = new ServerHandle(this);
            serverSend = new ServerSend(this);
            clients = new ServerClient[State.MaxClients + 1];
            userId.value = 0;
            
            packetHandlers = new Dictionary<byte, NetworkHandle.PacketHandler>()
            {
                { (byte)Packets.debugMessage, serverHandle.DebugMessage },
                
                { (byte)Packets.clientSettings, serverHandle.ClientSettings },
                { (byte)Packets.clientUDPConnection, serverHandle.ClientUDPConnection },
                { (byte)Packets.clientUDPConnectionStatus, serverHandle.ClientUDPConnectionStatus },
                
                { (byte)Packets.clientContainerPacket, serverHandle.ClientContainerPacket },
                { (byte)Packets.userStatus, networkHandle.UserStatus },
                { (byte)Packets.userVoiceId, networkHandle.UserVoiceID },
                
                { (byte)Packets.userPos, networkHandle.UserPos },
            };

            startServerEvent.Register(StartServer);
            stopServerEvent.Register(StopServer);
            debugMessageEvent.Register(serverSend.DebugMessage);

            serverState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }
        
        private void Start()
        {
            foreach (KeyValuePair<byte, NetworkHandle.PacketHandler> pair in networkHandle.packetHandlers)
            {
                packetHandlers.Add(pair.Key, pair.Value);
            }
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

        public ServerClient GetClient(byte userID)
        {
            if (userID != 0)
            {
                return clients[userID];
            }
            
            Threader.RunOnMainThread(() =>
            {
                Debug.Log("SERVER: Cant get Client from Message with Server or wrong ID. What the fuck are you doing?");
            });

            return null;
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

        public void Send(ServerClient client, Packet packet, bool useUDP)
        {
            packet.PrepareForSend();
            if (!useUDP || !featureSettings.UPDSupport || !client.clientUdpSupport)
            {
                tcpServer.SendData(client, packet.ToArray(), packet.Length());
            }
            else
            {
                tcpServer.SendData(client, packet.ToArray(), packet.Length());
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