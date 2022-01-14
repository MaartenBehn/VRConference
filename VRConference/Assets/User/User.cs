using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Mathematics;
using UnityEngine;
using Voice;

namespace Engine.User
{
    public class User : MonoBehaviour
    {
        public byte id;

        // Only needed when Server
        public TcpClient socket;
        public NetworkStream stream;
        public byte[] receiveBuffer;
        public string ip;
        public IPEndPoint endPoint;

        public Dictionary<String, bool> features = new Dictionary<string, bool>();

        public byte voiceId;
        [HideInInspector] public AudioSource voiceAudioSource;

        public const float headHigth = 1.7f;
        [SerializeField] private GameObject modelRoot;
        [SerializeField] private GameObject modelHead;
        public bool isVR;

        private void Update()
        {
            if (voiceAudioSource == null)
            {
                tryFindAudioScource();
            }
        }

        public bool HasFeature(String name)
        {
            return features != null && features.ContainsKey(name) && features[name];
        }

        void tryFindAudioScource()
        {
            GameObject g = GameObject.Find("UniVoice Peer #" + voiceId);
            if (g == null) return;
            voiceAudioSource = g.GetComponent<AudioSource>();
            voiceAudioSource.transform.SetParent(transform);
            
            VoiceConnection.SetAudioSourceSettings(voiceAudioSource);
        }

        public void SetPosition(float3 headPos, Quaternion headDirection)
        {
            float bodyYPos = headPos.y - headHigth;
            if (bodyYPos < 0)
            {
                bodyYPos = 0;
            }

            modelRoot.transform.position = new Vector3(headPos.x, bodyYPos, headPos.z);
            modelHead.transform.position = headPos;
            
            modelRoot.transform.rotation = Quaternion.Euler(0, headDirection.eulerAngles.y, 0);
            modelHead.transform.rotation = headDirection;
        }
    }
}