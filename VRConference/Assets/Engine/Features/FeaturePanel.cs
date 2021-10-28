using System;
using Network.Both;
using UnityEngine;
using Utility;

namespace Engine
{
    public class FeaturePanel : MonoBehaviour
    {
        [SerializeField] private GameObject featureSybolPreFab;
        
        [SerializeField] private PublicEvent loadServerEvent;
        [SerializeField] private PublicEvent loadClientEvent;
        [SerializeField] private PublicBool isServer;

        // Network
        private FeatureSymbol networkFeatureSymbol;
        [SerializeField] private PublicInt networkFeatureState;

        // Voice 
        private FeatureSymbol voiceFeatureSymbol;
        [SerializeField] private PublicInt voiceFeatureState;

        private void Awake()
        {
            loadServerEvent.Register(Show);
            loadClientEvent.Register(Show);
        }

        private void Show()
        {
            networkFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            networkFeatureSymbol.SetText("Network");
            networkFeatureSymbol.SetFeatureState(networkFeatureState);

            voiceFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            voiceFeatureSymbol.SetText("Voice");
            voiceFeatureSymbol.SetFeatureState(voiceFeatureState);
        }
    }
}