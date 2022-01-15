using UnityEngine;
using Voice;

namespace Audio.AudioSpeaker
{
    public class AudioSpeaker : MonoBehaviour
    {
        public int speakerId;
        public AudioSource audioSource;

        private void Start()
        {
            speakerId = SpeakerController.instance.speakers.Count;
            SpeakerController.instance.speakers.Add(this);
            audioSource.loop = true;

            VoiceConnection.SetAudioSourceSettings(audioSource);
        }

        public void PlayClip(AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}