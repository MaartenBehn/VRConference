using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using Utility;
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
        public bool loaded;

        public byte voiceId;
        [HideInInspector] public AudioSource voiceAudioSource;

        private void Update()
        {
            if (voiceAudioSource == null)
            {
                tryFindAudioScource();
            }
        }

        void tryFindAudioScource()
        {
            GameObject g = GameObject.Find("UniVoice Peer #" + voiceId);
            if (g == null) return;
            voiceAudioSource = g.GetComponent<AudioSource>();
            voiceAudioSource.transform.SetParent(transform);
            
            VoiceConnection.SetAudioSourceSettings(voiceAudioSource);
        }
    }
}