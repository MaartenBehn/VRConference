using System;
using Unity.Mathematics;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Engine.User
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private float spawnBounds;

        [SerializeField] private PublicEventFloat3 updatePosEvent;

        private void Awake()
        {
            updatePosEvent.Raise(new float3(Random.value * spawnBounds, 0, Random.value * spawnBounds));
        }
    }
}