using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Engine.User
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float spawnBounds;

        [SerializeField] private PublicEventFloat3 updatePosEvent;

        private void Awake()
        {
            updatePosEvent.Raise(new float3(10, 0, 0));
        }
    }
}