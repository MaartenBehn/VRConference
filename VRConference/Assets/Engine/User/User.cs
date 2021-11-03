using System;
using UnityEngine;
using Utility;

namespace Engine.User
{
    public class User : MonoBehaviour
    {
        public byte id;
        public byte voiceId;
        public AudioSource voiceAudioSource;

        public void SetVoiceId(byte id)
        {
            voiceId = id;
            voiceAudioSource = GameObject.Find("UniVoice Peer #" + id).GetComponent<AudioSource>();
            voiceAudioSource.transform.SetParent(transform);
            voiceAudioSource.spatialBlend = 1;
            voiceAudioSource.minDistance = 10;
        }
    }
    
}