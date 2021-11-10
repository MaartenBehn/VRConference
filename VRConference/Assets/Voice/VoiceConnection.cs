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

        [SerializeField] private PublicBool isHost;
        [SerializeField] private PublicEvent startEvent;
        [SerializeField] private PublicEvent stopEvent;
        [SerializeField] private PublicInt featureState;

        [SerializeField] private PublicEvent loadingDone;
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
            
            startEvent.Register(StartServer);
            stopEvent.Register(StopServer);
            loadingDone.Register(() =>
            {
                voiceID.value = (byte) agent.Network.OwnID;
            });
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
        }
    
        private void StartServer()
        {
            featureState.value = (int) FeatureState.starting;
        
            Init();

            if (isHost.value)
            {
                agent.Network.HostChatroom("VRConference");
            }
            else
            {
                agent.Network.JoinChatroom("VRConference");
            }
        }
    
        private void StopServer()
        {
            featureState.value = (int) FeatureState.stopping;
            if (isHost.value)
            {
                agent.Network.CloseChatroom();
            }
            else
            {
                agent.Network.LeaveChatroom();
            }
            
        }
    }
}
