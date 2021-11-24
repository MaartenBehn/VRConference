using Unity.Mathematics;
using UnityEngine;

namespace Engine.Spawner
{
    public class Spawner : MonoBehaviour
    {
        public static Spawner instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public GameObject[] spawnablePreFabs;

        public void Spawn(int preFabId, float3 pos, byte byUser)
        {
        
        }
    }
}
