using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using UnityEngine;
using Utility;

public class Voice : MonoBehaviour
{
    ChatroomAgent agent;

    [SerializeField] private PublicEvent startVoiceServer;
    [SerializeField] private PublicEvent stopVoiceServer;
    [SerializeField] private PublicEvent connectVoiceClient;
    [SerializeField] private PublicEvent disconnectVoiceClient;
    [SerializeField] private PublicEventBool voiceDone;
    
    private void Init()
    {
        agent = new InbuiltChatroomAgentFactory("ws://84.186.214.53:11002").Create();

        startVoiceServer.Register(StartServer);
        stopVoiceServer.Register(StopServer);
        connectVoiceClient.Register(ConnectClient);
        disconnectVoiceClient.Register(DisconnectClient);
        
        
        agent.Network.OnCreatedChatroom += () => {
            Debug.Log("VOICE: Room created");
            voiceDone.Raise(true);
        };

        agent.Network.OnChatroomCreationFailed += ex => {
            Debug.Log("VOICE: Room creation failed");
            voiceDone.Raise(false);
        };

        agent.Network.OnlosedChatroom += () => {
            Debug.Log("VOICE: Room closed");
        };

        // JOINING
        agent.Network.OnJoinedChatroom += id => {
            Debug.Log("VOICE: Peer "+ id + " joined");
            voiceDone.Raise(true);
        };

        agent.Network.OnChatroomJoinFailed += ex => {
            Debug.Log(ex);
            voiceDone.Raise(false);
        };

        agent.Network.OnLeftChatroom += () => {
            Debug.Log("VOICE: Peer left");
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
        Init();
        agent.Network.HostChatroom("VRConference"); 
    }
    
    private void StopServer()
    {
        agent.Network.CloseChatroom();
    }
    
    private void ConnectClient()
    {
        Init();
        agent.Network.JoinChatroom("VRConference");
    }
    
    private void DisconnectClient()
    {
        agent.Network.LeaveChatroom();
    }
}
