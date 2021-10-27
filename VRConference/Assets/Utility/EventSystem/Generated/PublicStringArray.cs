using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicStringArray", menuName = "Utility/PublicStringArray")]
    public class PublicStringArray : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] public string[] value;
        [SerializeField] private string[] initalValue;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        { 
            value = new string[initalValue.Length];
           for (int i = 0; i < initalValue.Length; i++)
           {
               value[i] = initalValue[i];
           }
        }
        
        // Debug
        [SerializeField] private string description;
    }
}