using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicEventFloat4", menuName = "Utility/PublicEventFloat4")]
    public class PublicEventFloat4 : ScriptableObject
    {
        [NonSerialized] private Action<float4>[] funcs = new Action<float4>[1];
        [NonSerialized] private int maxId = 0;
        [NonSerialized] private List<int> freeIds = new List<int>();
        
        public void Raise(float4 variable)
        {
            foreach (Action<float4> func in funcs)
            {
                func?.Invoke(variable);
            }
        }
    
        public int Register(Action<float4> func)
        {
            int id;
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
    
        public void Unregister(int id)
        {
            freeIds.Add(id);
            funcs[id] = null;
        }
    
        private void raiseArray()
        {
            int length = funcs.Length;
            Action<float4>[] newFunc = new Action<float4>[length + 1];
            
            for (int i = 0; i < length; i++)
            {
                newFunc[i] = funcs[i];
            }

            funcs = newFunc;
        }
        
        // Debug
        [SerializeField] private string description;
    }
}
