using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat2", menuName = "Utility/PublicFloat2")]
    public class PublicFloat2 : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public float2 value;
        [SerializeField] private float2 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}