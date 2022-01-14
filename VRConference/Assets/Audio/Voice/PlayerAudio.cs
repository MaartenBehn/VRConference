using Adrenak.UniMic;
using Engine;
using UnityEngine;
using Utility;

namespace Voice
{
    public class PlayerAudio : MonoBehaviour
    {
        public static PlayerAudio instance;

        [HideInInspector] public Mic mic;
        
        [HideInInspector] public AudioSource voiceAudioSource;
        public PublicFloat voiceSpecialBlend;
        public PublicInt voicemaxDist;
        public PublicInt voiceminDist;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        [SerializeField] private PublicInt voicefeatureState;
        
        private void Start()
        {
            if (voicefeatureState.value != (int) FeatureState.online) { return; }
            
            mic = GameObject.Find("UniMic.Mic").GetComponent<Mic>();
            mic.transform.SetParent(transform);
            voiceAudioSource = mic.GetComponent<AudioSource>();
            voiceAudioSource.spatialBlend = voiceSpecialBlend.value;
            voiceAudioSource.minDistance = voiceminDist.value;
            voiceAudioSource.maxDistance = voicemaxDist.value;
        }
    }
}