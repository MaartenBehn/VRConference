using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Utility
{
    [CreateAssetMenu(fileName = "PublicEventMaster", menuName = "Utility/PublicEventMaster")]
    public class PublicEventMaster : ScriptableObject
    {
        [NonSerialized] private Action<object>[] funcs = new Action<object>[1];
        [NonSerialized] private int maxId = 0;
        [NonSerialized] private List<int> freeIds = new List<int>();
        
        public void Raise(object variable)
        {
            foreach (Action<object> func in funcs)
            {
                func?.Invoke(variable);
            }
        }
    
        public int Register(Action<object> func)
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
            Action<object>[] newFunc = new Action<object>[length + 1];
            
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
