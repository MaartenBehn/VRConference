using System.Threading;
using Engine;
using Engine.Player;
using UnityEngine;
using Utility;

namespace Network
{
    public class NetworkController : MonoBehaviour
    {
        [SerializeField] public PublicBool isServer;

        [SerializeField] private PublicEvent startNetworkEvent;
        [SerializeField] private PublicEvent stopNetworkEvent;
        
        [SerializeField] private PublicEvent startUDPEvent;
        [SerializeField] private PublicEvent stopUDPEvent;
        
        public PublicByte userId;

        [SerializeField] private GameObject serverPreFab;
        [SerializeField] private GameObject clientPreFab;
        
        [HideInInspector] public Server.Server server;
        [HideInInspector] public Client.Client client;
        
        public NetworkHandle networkHandle;
        public NetworkSend networkSend;
        
        public FeatureSettings featureSettings;
        public PublicInt networkFeatureState;
        
        private void Awake()
        {
            networkHandle = new NetworkHandle(this);
            networkSend = new NetworkSend(this);
            
            startNetworkEvent.Register(() =>
            {
                Threader.RunOnMainThread(() =>
                {
                    if (isServer.value)
                    {
                        server = Instantiate(serverPreFab, transform).GetComponent<Server.Server>();
                        server.networkHandle = networkHandle;
                        server.networkSend = networkSend;
                        server.StartServer();
                    }
                    else
                    {
                        client = Instantiate(clientPreFab, transform).GetComponent<Client.Client>();
                        client.networkHandle = networkHandle;
                        client.networkSend = networkSend;
                        client.Connect();
                    }
                });
            });
            stopNetworkEvent.Register(() =>
            {
                if (isServer.value)
                {
                    server.StopServer();
                    Destroy(server.gameObject);
                }
                else
                {
                    client.Disconnect();
                    Destroy(client.gameObject);
                }
            });

            startUDPEvent.Register(() =>
            {
                if (isServer.value)
                {
                    server.udpServer.Start();
                }
                else
                {
                    client.udpClient.Connect();
                }
            });
            
            stopUDPEvent.Register(() =>
            {
                if (isServer.value)
                {
                    server.udpServer.Stop();
                }
                else
                {
                    client.udpClient.Disconnect();
                }
            });

            sendPosEvent.Register(networkSend.UserPos);
            sendVoiceId.Register(() =>
            {
                networkSend.UserVoiceID(true);
            });
        }

        public PublicEvent sendVoiceId;
        public PublicByte voiceId;
        public PublicEventFloat3 sendPosEvent;
    }
}