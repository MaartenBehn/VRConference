using System.Threading;
using Engine.Player;
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

        loadEvent.Register(Load);
        unloadEvent.Register(Unload);
    }

    [SerializeField] private PublicEventBool loadEvent;
    [SerializeField] private PublicEvent unloadEvent;
    [SerializeField] private PublicBool isHost;
    [SerializeField] private PublicByte userId;
    
    [SerializeField] private PublicEvent loadingDone;
    [SerializeField] private float timeOutLength = 30;
    [SerializeField] private PublicEvent loadingFailed;
    public FeatureSettings featureSettings;
    
    private void Load(bool b)
    {
        isHost.value = b;
        userId.value = b ? (byte)0 : (byte)1;
        Debug.Log("Loading");
        
        foreach (FeatureSettings.Feature feature in featureSettings.features)
        {
            if (!feature.active){continue;}
            
            Threader.RunAsync(() =>
            {
                WaitForDependencies(feature);
                Threader.RunOnMainThread(feature.startEvent.Raise);
            });
        }

        WaitForLoading();
    }
    
    private void WaitForDependencies(FeatureSettings.Feature feature)
    {
        bool waiting = true;
        while (waiting)
        {
            waiting = false;
            foreach (string dependicy in feature.dependicies)
            {
                if (featureSettings.Get(dependicy).featureState.value != (int) FeatureState.online)
                {
                    waiting = true;
                }
            }
            Thread.Sleep(100);
        }
    }
    
    private void Unload()
    {
        Debug.Log("Unloading");
        foreach (FeatureSettings.Feature feature in featureSettings.features)
        {
            feature.stopEvent.Raise();
        }
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
                    bool failed = false;
                    bool done = true;

                    foreach (FeatureSettings.Feature feature in featureSettings.features)
                    {
                        if (feature.featureState.value != (int) FeatureState.online)
                        {
                            done = false;
                        }
                        
                        if (feature.featureState.value == (int) FeatureState.failed)
                        {
                            failed = true;
                        }
                    }

                    if (done)
                    {
                        loadingDone.Raise();
                        loading = false;
                    }
                    
                    if (failed || Time.time >= startTime + timeOutLength)
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
