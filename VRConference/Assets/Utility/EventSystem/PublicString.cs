using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicString", menuName = "Utility/PublicString")]
    public class PublicString : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public string value;
        [SerializeField] private string initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}