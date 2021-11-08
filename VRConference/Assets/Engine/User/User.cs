using UnityEngine;

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
        }

        private void Update()
        {
            if (voiceAudioSource == null)
            {
                tryFindAudioScource();
            }
        }

        void tryFindAudioScource()
        {
            voiceAudioSource = GameObject.Find("UniVoice Peer #" + voiceId).GetComponent<AudioSource>();
            if (voiceAudioSource == null) return;
            voiceAudioSource.transform.SetParent(transform);
            voiceAudioSource.spatialBlend = 1;
            voiceAudioSource.minDistance = 10;
        }
    }
}