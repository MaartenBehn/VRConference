using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using UnityEngine;
using Utility;

namespace Voice
{
    public class VoiceConnection : MonoBehaviour
    {
        public static VoiceConnection instance;
        
        public ChatroomAgent agent;

        [SerializeField] private PublicString signalServerIP;
        [SerializeField] private PublicInt signalServerPort;

        [SerializeField] private PublicEvent startVoiceServer;
        [SerializeField] private PublicEvent stopVoiceServer;
        [SerializeField] private PublicEvent connectVoiceClient;
        [SerializeField] private PublicEvent disconnectVoiceClient;
        [SerializeField] private PublicInt featureState;
        
        [SerializeField] private PublicByte voiceID;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            startVoiceServer.Register(StartServer);
            stopVoiceServer.Register(StopServer);
            connectVoiceClient.Register(ConnectClient);
            disconnectVoiceClient.Register(DisconnectClient);
        }

        private void Init()
        {
            string uri = "ws://" + signalServerIP.value + ":" + signalServerPort.value;
            agent = new InbuiltChatroomAgentFactory(uri).Create();

            agent.Network.OnCreatedChatroom += () => {
                Debug.Log("VOICE: Room created");
                featureState.value = (int) FeatureState.online;
            };

            agent.Network.OnChatroomCreationFailed += ex => {
                Debug.Log("VOICE: Room creation failed");
                featureState.value = (int) FeatureState.failed;
            };

            agent.Network.OnlosedChatroom += () => {
                Debug.Log("VOICE: Room closed");
                featureState.value = (int) FeatureState.offline;
            };

            // JOINING
            agent.Network.OnJoinedChatroom += id => {
                Debug.Log("VOICE: Joined Chatroom "+ id);
                featureState.value = (int) FeatureState.online;
            };

            agent.Network.OnChatroomJoinFailed += ex => {
                Debug.Log(ex);
                featureState.value = (int) FeatureState.failed;
            };

            agent.Network.OnLeftChatroom += () => {
                Debug.Log("VOICE: Left Chatroom");
                featureState.value = (int) FeatureState.offline;
            };

            // PEERS
            agent.Network.OnPeerJoinedChatroom += id => {
                Debug.Log("VOICE: Peer "+ id + " joined");
            };

            agent.Network.OnPeerLeftChatroom += id => {
                Debug.Log("VOICE: Peer "+ id + " left");
            };

            voiceID.value = (byte) agent.Network.OwnID;
        }
    
        private void StartServer()
        {
            Threader.RunOnMainThread(() =>
            {
                featureState.value = (int) FeatureState.starting;
        
                Init();
                agent.Network.HostChatroom("VRConference");
            });
        }
    
        private void StopServer()
        {
            Threader.RunOnMainThread(() =>
            {
                featureState.value = (int) FeatureState.stopping;
                agent.Network.CloseChatroom();
            });
            
        }
    
        private void ConnectClient()
        {
            Threader.RunOnMainThread(() =>
            {
                featureState.value = (int) FeatureState.starting;
        
                Init();
                agent.Network.JoinChatroom("VRConference");
            });
        }
    
        private void DisconnectClient()
        {
            Threader.RunOnMainThread(() =>
            {
                featureState.value = (int) FeatureState.stopping;
                agent.Network.LeaveChatroom();
            });
        }
    }
}
