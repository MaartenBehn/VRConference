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
        public PublicByte clientId;
        public PublicInt clientState;
        
        private delegate void PacketHandler(byte userID, Packet packet);
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

        public PublicEvent loadingDone;
        public PublicEventByte userJoined;
        public PublicEventByte userLeft;

        [SerializeField] private PublicByte voiceID;

        private void Awake()
        {
            tcpClient = new TCPClient(this);
            udpClient = new UDPClient(this);
            clientHandle = new ClientHandle(this);
            clientSend = new ClientSend(this);
            
            // All Handle funcs are mapped here to the packet Type. They should have the same name.
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.debugMessage, clientHandle.DebugMessage },
                
                { (byte)Packets.serverSettings, clientHandle.ServerSettings },
                { (byte)Packets.serverStartUDP, clientHandle.ServerStartUDP },
                { (byte)Packets.serverUDPConnection, clientHandle.ServerUDPConnection },
                
                { (byte)Packets.userStatus, clientHandle.UserStatus },
            };
            
            // Init all Events
            connectEvent.Register(tcpClient.Connect);
            disconnectEvent.Register(Disconnect);
            debugMessageEvent.Register(clientSend.DebugMessage);

            loadingDone.Register(() =>
            {
                clientSend.UserStatus(1);
                clientSend.UserVoiceID(voiceID.value);
            });
            
            // Setting state
            clientState.value = (int) NetworkState.notConnected;
            networkFeatureState.value = (int) FeatureState.offline;
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        // This funcs gets the incoming bytes and maps it to the handle func.
        public void HandleData(byte[] data)
        {
            Packet packet = new Packet(data);
            packet.PrepareForRead();
            
            // This checks if the data has the length it should have.
            int length = packet.ReadInt32();
            if (length + 4 != data.Length)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("CLIENT: Packet size not correct.");
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
        
        public void SendTCPData(Packet packet)
        {
            packet.PrepareForSend();
            tcpClient.SendData(packet.ToArray(), packet.Length());
        }
        
        public void SendUDPData(Packet packet)
        {
            if (!featureSettings.UPDSupport || !serverUDPSupport)
            {
                SendTCPData(packet);
                return;
            }
            
            packet.PrepareForSend();
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