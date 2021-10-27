using System;
using System.Collections;
using System.Collections.Generic;
using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using UnityEngine;

public class Voice : MonoBehaviour
{
    ChatroomAgent agent;

    [SerializeField] private PublicEvent startVoiceServer;
    [SerializeField] private PublicEvent stopVoiceServer;
    [SerializeField] private PublicEvent connectVoiceClient;
    [SerializeField] private PublicEvent disconnectVoiceClient;

    private void Awake()
    {
        agent = new InbuiltChatroomAgentFactory("ws://127.0.0.1:12776").Create();

        startVoiceServer.Register(StartServer);
        stopVoiceServer.Register(StopServer);
        connectVoiceClient.Register(ConnectClient);
        disconnectVoiceClient.Register(DisconnectClient);
        
        
        agent.Network.OnCreatedChatroom += () => {
            Debug.Log("Room created");
        };

        agent.Network.OnChatroomCreationFailed += ex => {
            Debug.Log("Room creation failed");
        };

        agent.Network.OnlosedChatroom += () => {
            Debug.Log("Room closed");
        };

        // JOINING
        agent.Network.OnJoinedChatroom += id => {
            Debug.Log("Peer "+ id + "joined");
        };

        agent.Network.OnChatroomJoinFailed += ex => {
            Debug.Log(ex);
        };

        agent.Network.OnLeftChatroom += () => {
            Debug.Log("Peer left");
        };

        // PEERS
        agent.Network.OnPeerJoinedChatroom += id => {
            Debug.Log("Peer "+ id + "joined");
        };

        agent.Network.OnPeerLeftChatroom += id => {
            Debug.Log("Peer "+ id + "left");
        };
    }

    private void StartServer()
    {
        agent.Network.HostChatroom("VRConference"); 
    }
    
    private void StopServer()
    {
        agent.Network.CloseChatroom();
    }
    
    private void ConnectClient()
    {
        agent.Network.JoinChatroom("VRConference");
    }
    
    private void DisconnectClient()
    {
        agent.Network.LeaveChatroom();
    }
}
