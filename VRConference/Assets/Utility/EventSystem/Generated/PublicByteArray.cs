using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicByteArray", menuName = "Utility/PublicByteArray")]
    public class PublicByteArray : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public Byte[] value;
        [SerializeField] private Byte[] initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        { 
            value = new Byte[initalValue.Length]; 
            for (int i = 0; i < initalValue.Length; i++)
            {
                value[i] = initalValue[i];
            }
        }
        
        // Debug
        [SerializeField] private string description;
    }
}