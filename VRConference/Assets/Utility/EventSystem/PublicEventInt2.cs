using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicEventInt2", menuName = "Utility/PublicEventInt2")]
    public class PublicEventInt2 : ScriptableObject
    {
        [NonSerialized] private Action<int2>[] funcs = new Action<int2>[1];
        [NonSerialized] private int maxId = 0;
        [NonSerialized] private List<int> freeIds = new List<int>();
        
        public void Raise(int2 variable)
        {
            foreach (Action<int2> func in funcs)
            {
                func?.Invoke(variable);
            }
        }
    
        public int Register(Action<int2> func)
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
            Action<int2>[] newFunc = new Action<int2>[length + 1];
            
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
