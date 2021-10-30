using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat3", menuName = "Utility/PublicFloat3")]
    public class PublicFloat3 : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public float3 value;
        [SerializeField] private float3 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
           value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}