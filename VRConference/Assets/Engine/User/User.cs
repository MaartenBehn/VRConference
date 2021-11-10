using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Engine.User
{
    public class User : MonoBehaviour
    {
        public byte id;
        
        public Dictionary<String, bool> features;
        
        public byte voiceId;
        public AudioSource voiceAudioSource;

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
            voiceAudioSource.spatialBlend = 1;
            voiceAudioSource.minDistance = 10;
        }
    }
}