using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicIntArray", menuName = "Utility/PublicIntArray")]
    public class PublicIntArray : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public int[] value;
        [SerializeField] private int[] initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        { 
            value = new int[initalValue.Length]; 
            for (int i = 0; i < initalValue.Length; i++)
            {
                value[i] = initalValue[i];
            }
        }
        
        // Debug
        [SerializeField] private string description;
    }
}