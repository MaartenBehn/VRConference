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
        [SerializeField] private PublicEvent loadingDone;
        [SerializeField] private float speed;

        private void Awake()
        {
            loadingDone.Register(init);
            gameObject.SetActive(false);
        }

        void init()
        {
            gameObject.SetActive(true);
            updatePosEvent.Raise(new float3(10, 0, 0));
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, Time.deltaTime * speed);
        }
    }
}