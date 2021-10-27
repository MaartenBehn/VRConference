using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt4", menuName = "Utility/PublicInt4")]
    public class PublicInt4 : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public int4 value;
        [SerializeField] private int4 initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
           value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}