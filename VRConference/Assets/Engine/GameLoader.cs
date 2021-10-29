using System.Threading;
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

        loadServerEvent.Register(() =>
        {
            isServer.value = true;
            Load();
        });
        loadClientEvent.Register(() =>
        {
            isServer.value = false;
            Load();
        });
        
        unloadEvent.Register(Unload);
    }

    [SerializeField] private PublicEvent loadServerEvent;
    [SerializeField] private PublicEvent loadClientEvent;
    [SerializeField] private PublicEvent unloadEvent;
    [SerializeField] private PublicBool isServer;
    
    // Network
    [SerializeField] private PublicEvent startServerEvent;
    [SerializeField] private PublicEvent stopServerEvent;
    [SerializeField] private PublicEvent connectClientEvent;
    [SerializeField] private PublicEvent disconnectClientEvent;
    [SerializeField] private PublicInt networkFeatureState;

    // Voice
    [SerializeField] private PublicEvent startVoiceServerEvent;
    [SerializeField] private PublicEvent stopVoiceServerEvent;
    [SerializeField] private PublicEvent connectVoiceClientEvent;
    [SerializeField] private PublicEvent disconnectVoiceClientEvent;
    [SerializeField] private PublicInt voiceFeatureState;

    [SerializeField] private PublicEvent loadingDone;
    [SerializeField] private float timeOutLength = 30;
    [SerializeField] private PublicEvent loadingFailed;

    private void Load()
    {
        Threader.RunAsync(() =>
        {
            if (isServer.value)
            {
                startServerEvent.Raise();
                startVoiceServerEvent.Raise();
            }
            else
            {
                connectClientEvent.Raise();
                connectVoiceClientEvent.Raise();
            }
        });
        WaitForLoading();
    }
    
    private void Unload()
    {
        Threader.RunAsync(() =>
        {
            if (isServer.value)
            {
                stopVoiceServerEvent.Raise();
                stopServerEvent.Raise();
            }
            else
            {
                disconnectClientEvent.Raise();
                disconnectVoiceClientEvent.Raise();
            }
        });
    }

    private void WaitForLoading()
    {
        float startTime = Time.time;
        Threader.RunAsync(() =>
        {
            bool loading = true;
            while (loading)
            {
                Threader.RunOnMainThread(() =>
                {
                    Debug.Log("Loading");
                    
                    if (networkFeatureState.value == (int) FeatureState.online &&
                        voiceFeatureState.value == (int) FeatureState.online)
                    {
                        loadingDone.Raise();
                        loading = false;
                    }
                    
                    if (Time.time >= startTime + timeOutLength)
                    {
                        Unload();
                        loadingFailed.Raise();
                        loading = false;
                    }
                });
                Thread.Sleep(1000);
            }
        });
    }
}
