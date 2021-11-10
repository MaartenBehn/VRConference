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

        [HideInInspector] public Mic mic;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            mic = GameObject.Find("UniMic.Mic").GetComponent<Mic>();
            mic.transform.SetParent(transform);
        }

        private void FixedUpdate()
        {
            //updatePosEvent.Raise(transform.position);
        }
    }
}