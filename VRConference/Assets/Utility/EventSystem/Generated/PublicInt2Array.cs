using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicInt2Array", menuName = "Utility/PublicInt2Array")]
    public class PublicInt2Array : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public int2[] value;
        [SerializeField] private int2[] initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        { 
            value = new int2[initalValue.Length]; 
            for (int i = 0; i < initalValue.Length; i++)
            {
                value[i] = initalValue[i];
            }
        }
        
        // Debug
        [SerializeField] private string description;
    }
}