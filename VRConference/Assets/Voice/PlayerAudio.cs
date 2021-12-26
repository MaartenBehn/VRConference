using Adrenak.UniMic;
using UnityEngine;
using Utility;

namespace Voice
{
    public class PlayerAudio : MonoBehaviour
    {
        public static PlayerAudio instance;

        //[SerializeField] private PublicEventFloat3 updatePosEvent;

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
        
        /*
        private float3 lastPos;
        private void FixedUpdate()
        {
            float3 pos = transform.position;
            if (lastPos.x != pos.x || lastPos.y != pos.y || lastPos.z != pos.z)
            {
                updatePosEvent.Raise(pos);
                lastPos = pos;
            }
        }
        */
    }
}