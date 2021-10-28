using UnityEngine;
using Utility;

public enum FeatureState
{
    offline = 0,
    starting = 1,
    online = 2,
    failed = 3,
    stopping = 4
}

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;
    
    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); return; }

        loadServerEvent.Register(LoadServer);
        loadClientEvent.Register(LoadClient);
        unloadServerEvent.Register(UnloadServer);
        unloadClientEvent.Register(UnloadClient);
    }

    [SerializeField] private PublicEvent loadServerEvent;
    [SerializeField] private PublicEvent loadClientEvent;
    [SerializeField] private PublicEvent unloadServerEvent;
    [SerializeField] private PublicEvent unloadClientEvent;
    [SerializeField] private PublicBool isServer;
    
    // Network
    [SerializeField] private PublicEvent startServerEvent;
    [SerializeField] private PublicEvent stopServerEvent;
    [SerializeField] private PublicEvent connectClientEvent;
    [SerializeField] private PublicEvent disconnectClientEvent;

    // Voice
    [SerializeField] private PublicEvent startVoiceServerEvent;
    [SerializeField] private PublicEvent stopVoiceServerEvent;
    [SerializeField] private PublicEvent connectVoiceClientEvent;
    [SerializeField] private PublicEvent disconnectVoiceClientEvent;

    private void LoadServer()
    {
        isServer.value = true;
        startServerEvent.Raise();
        startVoiceServerEvent.Raise();
    }
    
    private void UnloadServer()
    {
        stopVoiceServerEvent.Raise();
        stopServerEvent.Raise();
    }
    
    private void LoadClient()
    {
        isServer.value = false;
        connectClientEvent.Raise();
        connectVoiceClientEvent.Raise();
    }
    
    private void UnloadClient()
    {
        disconnectVoiceClientEvent.Raise();
        disconnectClientEvent.Raise();
    }
}
