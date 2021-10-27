using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt", menuName = "Utility/PublicInt")]
    public class PublicInt : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public int value;
        [SerializeField] private int initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            value = initalValue;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}