using System.Collections.Generic;
using System.Linq;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client
{
    public class Client : MonoBehaviour
    {
        public PublicString ip;
        public PublicInt clientTCPPort;
        public PublicInt clientUDPPort;
        public PublicInt serverTCPPort;
        public PublicInt serverUDPPort;
        
        public PublicByte clientId;

        private static Dictionary<byte, NetworkHandle.PacketHandler> packetHandlers;

        public TCPClient tcpClient;
        public UDPClient udpClient;
        public ClientHandle clientHandle;
        public ClientSend clientSend;
        
        public PublicInt networkFeatureState;
        public PublicInt udpFeatureState;

        public NetworkSend networkSend;
        public NetworkHandle networkHandle;
        
        [SerializeField] private PublicEvent unloadEvent;

        private void Awake()
        {
            tcpClient = new TCPClient(this);
            udpClient = new UDPClient(this);
            clientHandle = new ClientHandle(this);
            clientSend = new ClientSend(this);
            
            // All Handle funcs are mapped here to the packet Type. They should have the same name.
            packetHandlers = new Dictionary<byte, NetworkHandle.PacketHandler>()
            {
                { (byte)Packets.debugMessage, clientHandle.DebugMessage },
                { (byte)Packets.serverInit, clientHandle.ServerInit },
                { (byte)Packets.serverUserJoined, clientHandle.ServerUserJoined },
                { (byte)Packets.serverUserLeft, clientHandle.ServerUserLeft },
                { (byte)Packets.serverUDPConnection, clientHandle.ServerUDPConnection },
            };

            networkFeatureState.value = (int) FeatureState.offline;
        }

        public void Connect()
        {
            if (networkFeatureState.value != (int) FeatureState.offline) {return;}
            
            networkFeatureState.value = (int) FeatureState.starting;
            tcpClient.Connect();
        }

        private void Start()
        {
            foreach (KeyValuePair<byte, NetworkHandle.PacketHandler> pair in networkHandle.packetHandlers)
            {
                packetHandlers.Add(pair.Key, pair.Value);
            }
        }

        // This funcs gets the incoming bytes and maps it to the handle func.
        // Main Thread
        public void HandleData(byte[] data)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            bool async = packet.ReadBool();
            // This checks if the data has the length it should have.
            int length = packet.ReadInt32();
            if (length + 5 > data.Length)
            {
                Debug.Log("CLIENT: Packet size not correct.");
                return;
            }
            
            byte packetId = packet.ReadByte();
            byte userID = packet.ReadByte();
            
            packetHandlers[packetId](userID, packet);
            
            if (length + 5 < data.Length)
            {
                var list = data.ToList().GetRange( length + 5, data.Length - (length + 5));
                HandleData(list.ToArray());
            }
        }
        
        // Async
        public void Send(Packet packet, bool useUDP, bool async)
        {
            packet.PrepareForSend(async);
            
            if (!useUDP || udpFeatureState.value != (int) FeatureState.online)
            {
                tcpClient.SendData(packet.ToArray(), packet.Length());
            }
            else
            {
                udpClient.SendData(packet.ToArray(), packet.Length());
            }
        }
        
        // Main Thread
        public void Disconnect()
        {
            if (networkFeatureState.value != (int) FeatureState.online) {return;}
            
            Debug.Log("CLIENT: Disconnecting...");
            networkFeatureState.value = (int) FeatureState.stopping;

            tcpClient.Disconnect();
            udpClient.Disconnect();

            Debug.Log("CLIENT: Disconnected");
            networkFeatureState.value = (int) FeatureState.offline;
            
            unloadEvent.Raise();
        }
    }
}
