using UnityEngine;
using Utility;

namespace Engine.Features
{
    public class FeaturePanel : MonoBehaviour
    {
        [SerializeField] private GameObject featureSybolPreFab;

        // Player
        private FeatureSymbol playerFeatureSymbol;
        [SerializeField] private PublicInt playerFeatureState;
        
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
            playerFeatureSymbol = Instantiate(featureSybolPreFab, transform).GetComponent<FeatureSymbol>();
            playerFeatureSymbol.SetText("Player");
            playerFeatureSymbol.SetFeatureState(playerFeatureState);
            
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