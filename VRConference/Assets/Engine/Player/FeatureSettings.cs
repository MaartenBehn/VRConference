using System;
using UnityEngine;
using Utility;

namespace Engine.Player
{
    
    
    [CreateAssetMenu(fileName = "FeatureSettings", menuName = "Feature/FeatureSettings")]
    public class FeatureSettings : ScriptableObject
    {
        [Serializable]
        public struct Feature
        {
            public String name;
            public bool active;
            public PublicEvent startEvent;
            public PublicEvent stopEvent;
            public PublicInt featureState;
            public String[] dependicies;
        }
        public Feature[] features;
        
        public Feature Get(String name){
            for (int i = 0; i < features.Length; i++)
            {
                if (features[i].name == name)
                {
                    return features[i];
                }
            }
            
            Debug.LogError("Feature doesn't exist");
            return features[0];
        }
    }
}

