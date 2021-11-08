using System;
using Adrenak.UniMic;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Engine.User
{
    public class Player : MonoBehaviour
    {
        public static Player instance;
        
        [SerializeField] private float spawnBounds;

        [SerializeField] private PublicEventFloat3 updatePosEvent;

        public Mic mic;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            updatePosEvent.Raise(new float3(10, 0, 0));
            mic = GameObject.Find("UniMic.Mic").GetComponent<Mic>();
            mic.transform.SetParent(transform);
        }
    }
}