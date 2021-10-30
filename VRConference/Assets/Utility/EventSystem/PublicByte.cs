using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicByte", menuName = "Utility/PublicByte")]
    public class PublicByte : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public byte value;
        [SerializeField] private byte initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}