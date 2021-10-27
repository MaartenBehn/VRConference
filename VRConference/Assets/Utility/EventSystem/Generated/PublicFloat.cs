using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat", menuName = "Utility/PublicFloat")]
    public class PublicFloat : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public float value;
        [SerializeField] private float initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}