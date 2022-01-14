using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Engine.AudioSpeaker
{
    public class SpeakerController : MonoBehaviour
    {
        public static SpeakerController instance;

        public List<AudioSpeaker> speakers;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            speakers = new List<AudioSpeaker>();
        }
    }
}