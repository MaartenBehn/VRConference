using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicFloat2Array", menuName = "Utility/PublicFloat2Array")]
    public class PublicFloat2Array : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public float2[] value;
        [SerializeField] private float2[] initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        { 
            value = new float2[initalValue.Length]; 
            for (int i = 0; i < initalValue.Length; i++)
            {
                value[i] = initalValue[i];
            }
        }
        
        // Debug
        [SerializeField] private string description;
    }
}