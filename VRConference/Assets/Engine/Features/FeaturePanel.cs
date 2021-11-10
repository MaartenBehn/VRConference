using System;
using Network.Both;
using UnityEngine;
using Utility;

namespace Engine
{
    public class FeaturePanel : MonoBehaviour
    {
        [SerializeField] private GameObject featureSybolPreFab;

        // Network
        private FeatureSymbol networkFeatureSymbol;
        [SerializeField] private PublicInt networkFeatureState;
        
        // Network
        private FeatureSymbol udpFeatureSymbol;
        [SerializeField] private PublicInt udpFeatureState;

        // Voice 
        private FeatureSymbol voiceFeatureSymbol;
        [SerializeField] private PublicInt voiceFeatureState;
        
        private void Start()
        {
            networkFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            networkFeatureSymbol.SetText("Network");
            networkFeatureSymbol.SetFeatureState(networkFeatureState);
            
            udpFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            udpFeatureSymbol.SetText("UDP");
            udpFeatureSymbol.SetFeatureState(udpFeatureState);

            voiceFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            voiceFeatureSymbol.SetText("Voice");
            voiceFeatureSymbol.SetFeatureState(voiceFeatureState);
        }
    }
}