using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicEventByte", menuName = "Utility/PublicEventByte")]
    public class PublicEventByte : ScriptableObject
    {
        [NonSerialized] private Action<byte>[] funcs = new Action<byte>[1];
        [NonSerialized] private byte maxId = 0;
        [NonSerialized] private List<byte> freeIds = new List<byte>();
        
        public void Raise(byte variable)
        {
            foreach (Action<byte> func in funcs)
            {
                func?.Invoke(variable);
            }
        }
    
        public byte Register(Action<byte> func)
        {
            byte id;
            if (freeIds.Count == 0)
            {
                id = maxId;
                maxId++;
            }
            else
            {
                id = freeIds[0];
                freeIds.RemoveAt(0);
            }
            
            if (funcs.Length <= id)
            {
                raiseArray();
            }
            
            funcs[id] = func;
            return id;
        }
    
        public void Unregister(byte id)
        {
            freeIds.Add(id);
            funcs[id] = null;
        }
    
        private void raiseArray()
        {
            int length = funcs.Length;
            Action<byte>[] newFunc = new Action<byte>[length + 1];
            
            for (byte i = 0; i < length; i++)
            {
                newFunc[i] = funcs[i];
            }

            funcs = newFunc;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}
