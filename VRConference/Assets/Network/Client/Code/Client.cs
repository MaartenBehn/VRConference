using System.Collections.Generic;
using Network.Both;
using UnityEngine;
using Utility;

namespace Network.Client.Code
{
    public class Client : MonoBehaviour
    {
        public PublicString ip;
        public PublicInt clientPort;
        public PublicInt serverPort;
        public PublicInt clientId;
        public PublicInt clientState;
        
        private delegate void PacketHandler(Packet packet);
        private static Dictionary<byte, PacketHandler> packetHandlers;

        public TCPClient tcpClient;
        public UDPClient udpClient;
        public ClientHandle clientHandle;
        public ClientSend clientSend;
        
        [SerializeField] private PublicEvent connectEvent;
        [SerializeField] private PublicEvent disconnectEvent;
        [SerializeField] private PublicEventString debugMessageEvent;

        public NetworkFeatureSettings featureSettings;
        public PublicInt networkFeatureState;

        [HideInInspector] public bool serverUDPSupport;
        public PublicBool udpOnline;

        private void Awake()
        {
            tcpClient = new TCPClient(this);
            udpClient = new UDPClient(this);
            clientHandle = new ClientHandle(this);
            clientSend = new ClientSend(this);
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.debugMessage, clientHandle.DebugMessage },
                
                { (byte)Packets.serverSettings, clientHandle.ServerSettings },
                { (byte)Packets.serverStartUDP, clientHandle.ServerStartUDP },
                { (byte)Packets.serverUDPConnection, clientHandle.ServerUDPConnection },
            };

            connectEvent.Register(tcpClient.Connect);
            disconnectEvent.Register(Disconnect);
            debugMessageEvent.Register(clientSend.DebugMessage);

            clientState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void HandleData(byte[] data)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            int length = packet.ReadInt32();
            if (length + 4 != data.Length)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Packet size not correct.");
                });
                return;
            }
            
            byte serverId = packet.ReadByte();
            if (serverId != 1)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Server ID not correct.");
                });
                return;
            }
            
            byte packetId = packet.ReadByte();

            Threader.RunOnMainThread(() =>
            {
                packetHandlers[packetId](packet);
            });
        }

        private Packet AddHeaderToPacket(Packet packet)
        {
            packet.Write(0, (byte) clientId.value);
            packet.Write(0, packet.Length());
            return packet;
        }

        public void SendTCPData(Packet packet)
        {
            packet = AddHeaderToPacket(packet);
            tcpClient.SendData(packet.ToArray(), packet.Length());
        }
        
        public void SendUDPData(Packet packet)
        {
            if (!featureSettings.UPDSupport || !serverUDPSupport)
            {
                SendTCPData(packet);
                return;
            }
            
            packet = AddHeaderToPacket(packet);
            udpClient.SendData(packet.ToArray(), packet.Length());
        }

        public void Disconnect()
        {
            if (clientState.value != (int) NetworkState.connected) {return;}
            
            Debug.Log("CLIENT: Disconnecting...");
            clientState.value = (int) NetworkState.disconnecting;
            networkFeatureState.value = (int) FeatureState.stopping;

            tcpClient.Disconnect();
            udpClient.Disconnect();

            Debug.Log("CLIENT: Disconnected");
            clientState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }
    }
}