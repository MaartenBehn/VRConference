using System;
using Network.Both;
using Unity.VisualScripting;
using UnityEngine;
using Utility;

enum LoaderState
{
    notLoaded = 1,
    network = 2,
    networkDone = 3,
    voice = 4,
    voiceDone = 5,
}

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); return; }

        state = LoaderState.notLoaded;
        
        voiceDone.Register(b =>
        {
            if (state == LoaderState.voice)
            {
                state = LoaderState.voiceDone;
            }
        });
        
        loadServerEvent.Register(() =>
        {
            Threader.RunAsync(LoadServer);
        });
        
        loadClientEvent.Register(() =>
        {
            Threader.RunAsync(LoadClient);
        });
    }

    private LoaderState state;
    
    [SerializeField] private PublicEvent loadServerEvent;
    [SerializeField] private PublicEvent loadClientEvent;

    // Network
    [SerializeField] private PublicEvent startServerEvent;
    [SerializeField] private PublicEvent stopServerEvent;
    [SerializeField] private PublicEvent connectClientEvent;
    [SerializeField] private PublicEvent disconnectClientEvent;
    [SerializeField] private PublicInt serverState;
    [SerializeField] private PublicInt clientState;

    // Voice
    [SerializeField] private PublicEvent startVoiceServerEvent;
    [SerializeField] private PublicEvent stopVoiceServerEvent;
    [SerializeField] private PublicEvent connectVoiceClientEvent;
    [SerializeField] private PublicEvent disconnectVoiceClientEvent;
    [SerializeField] private PublicEventBool voiceDone;

    private void LoadServer()
    {
        while (true)
        {
            switch (state)
            {
                case LoaderState.notLoaded:
                    state = LoaderState.network;
                    startServerEvent.Raise();
                    break;
                
                case LoaderState.network:
                    if (serverState.value == (int) NetworkState.connected)
                    {
                        state = LoaderState.networkDone;
                    }
                    break;
                
                case LoaderState.networkDone:
                    state = LoaderState.voice;
                    startVoiceServerEvent.Raise();
                    break;
                
                case LoaderState.voiceDone:
                    return;
            }
        }
    }
    
    private void LoadClient()
    {
        while (true)
        {
            switch (state)
            {
                case LoaderState.notLoaded:
                    state = LoaderState.network;
                    connectClientEvent.Raise();
                    break;
                
                case LoaderState.network:
                    if (serverState.value == (int) NetworkState.connected)
                    {
                        state = LoaderState.networkDone;
                    }
                    break;
                
                case LoaderState.networkDone:
                    state = LoaderState.voice;
                    connectVoiceClientEvent.Raise();
                    break;
                
                case LoaderState.voiceDone:
                    return;
            }
        }
    }
}
