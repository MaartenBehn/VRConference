using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt2", menuName = "Utility/PublicInt2")]
    public class PublicInt2 : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public int2 value;
        [SerializeField] private int2 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}