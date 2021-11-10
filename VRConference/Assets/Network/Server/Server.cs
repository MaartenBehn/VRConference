using System.Collections.Generic;
using Engine;
using Network.Both;
using Network.Server.Code;
using UnityEngine;
using Utility;

namespace Network.Server
{
    public class Server : MonoBehaviour
    {
        public PublicInt port;
        public NetworkState serverState;

        private static Dictionary<byte, NetworkHandle.PacketHandler> packetHandlers;

        public TCPServer tcpServer;
        public UDPServer udpServer;
        public ServerHandle serverHandle;
        public ServerSend serverSend;
        
        public ServerClient[] clients;
        
        public PublicInt networkFeatureState;
        public PublicInt udpFeatureState;
        
        public NetworkSend networkSend;
        public NetworkHandle networkHandle;

        private void Awake()
        {
            tcpServer = new TCPServer(this);
            udpServer = new UDPServer(this);
            serverHandle = new ServerHandle(this);
            serverSend = new ServerSend(this);
            clients = new ServerClient[State.MaxClients + 1];

            packetHandlers = new Dictionary<byte, NetworkHandle.PacketHandler>()
            {
                { (byte)Packets.debugMessage, serverHandle.DebugMessage },
                { (byte)Packets.clientContainerPacket, serverHandle.ClientContainerPacket },
                { (byte)Packets.clientStartUDP, serverHandle.ClientStartUDP },
                { (byte)Packets.clientUDPConnection, serverHandle.ClientUDPConnection },
            };
            
            serverState = NetworkState.notConnected;
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

        public void StartServer()
        {
            if (serverState != NetworkState.notConnected) { return; }
            
            Debug.Log("SERVER: Starting...");
            serverState = NetworkState.connecting;
            networkFeatureState.value = (int) FeatureState.starting;
            
            tcpServer.Start();

            Debug.Log("SERVER: Started");
            serverState = NetworkState.connected;
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
            if (!useUDP)
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

        public void StopServer()
        {
            if (serverState != NetworkState.connected) { return; }
            
            Debug.Log("SERVER: Stopping...");
            serverState = NetworkState.disconnecting;
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
            serverState = NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }
    }
}