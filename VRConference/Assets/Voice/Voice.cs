using System;
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
    [SerializeField] private PublicInt featureState;

    private void Awake()
    {
        startVoiceServer.Register(StartServer);
        stopVoiceServer.Register(StopServer);
        connectVoiceClient.Register(ConnectClient);
        disconnectVoiceClient.Register(DisconnectClient);
    }

    private void Init()
    {
        agent = new InbuiltChatroomAgentFactory("ws://84.186.214.53:11002").Create();
        
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
        agent.Network.HostChatroom("VRConference");
    }
    
    private void StopServer()
    {
        featureState.value = (int) FeatureState.stopping;
        agent.Network.CloseChatroom();
    }
    
    private void ConnectClient()
    {
        featureState.value = (int) FeatureState.starting;
        
        Init();
        agent.Network.JoinChatroom("VRConference");
    }
    
    private void DisconnectClient()
    {
        featureState.value = (int) FeatureState.stopping;
        agent.Network.LeaveChatroom();
    }
}
