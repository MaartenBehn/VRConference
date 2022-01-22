using System.Collections.Generic;
using System.Linq;
using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using Engine;
using UnityEngine;
using Users;
using Utility;
using Voice;

namespace Audio.Voice.VoiceClients
{
    // Used code from https://github.com/adrenak/univoice sample
    public class VoiceClientsPanel : MonoBehaviour
    {
        ChatroomAgent agent;
        Dictionary<short, UserEntry> peerViews = new Dictionary<short, UserEntry>();
        public Transform peerViewContainer;
        public UserEntry peerViewPreFab;
        [SerializeField] private PublicInt featureState;
        [SerializeField] private PublicEvent loadingDone;

        private void Awake()
        {
            void setup()
            {
                agent = VoiceConnection.instance.agent;

                UpdateList();
            }
            
            if (featureState.value != (int) FeatureState.online)
            {
                loadingDone.Register(setup);
                return;
            }
            setup();
        }

        void UpdateList()
        {
            for (int i = 0; i < peerViewContainer.transform.childCount; i++)
            {
                Destroy(peerViewContainer.transform.GetChild(i).gameObject);
            }
            peerViews.Clear();

            foreach (KeyValuePair<byte,User> keyValuePair in UserController.instance.users)
            {
                User user = keyValuePair.Value;
                UserEntry entry = Instantiate(peerViewPreFab, peerViewContainer);
                entry.nameText.text = user.name;

                if (user.voiceAudioSource != null)
                {
                    entry.IncomingAudio = agent.PeerSettings[user.voiceId].muteThem;
                    entry.OutgoingAudio = agent.PeerSettings[user.voiceId].muteSelf;

                    entry.OnIncomingModified += value =>
                        agent.PeerSettings[user.voiceId].muteThem = !value;

                    entry.OnOutgoingModified += value =>
                        agent.PeerSettings[user.voiceId].muteSelf = !value;

                    entry.SetPeerID(user.voiceId);
                }
                peerViews.Add(user.voiceId, entry);
            }
        }
        
        private int useres = 0;
        private float lastListUpdate;
        private void Update()
        {
            if (featureState.value != (int) FeatureState.online) { return; }
            
            if (useres != UserController.instance.users.Count || lastListUpdate + 2 < Time.time)
            {
                lastListUpdate = Time.time;
                UpdateList();
                useres = UserController.instance.users.Count;
            }

            foreach (KeyValuePair<short,UserEntry> keyValuePair in peerViews)
            {
                UserEntry view = keyValuePair.Value;
                short key = keyValuePair.Key;
                
                if (!agent.PeerOutputs.ContainsKey(key)) continue;
                AudioSource audioSource = (agent.PeerOutputs[key] as InbuiltAudioOutput)?.AudioSource;
                updateSpectrum(view, audioSource);
            }
        }

        void updateSpectrum(UserEntry view, AudioSource audioSource)
        {
            /*
            * This is an inefficient way of showing a part of the 
            * audio source spectrum. AudioSource.GetSpectrumData returns
            * frequency values up to 24000 Hz in some cases. Most human
            * speech is no more than 5000 Hz. Showing the entire spectrum
            * will therefore lead to a spectrum where much of it doesn't
            * change. So we take only the spectrum frequencies between
            * the average human vocal range.
            * 
            * Great source of information here: 
            * http://answers.unity.com/answers/158800/view.html
            */
            var size = 512;
            var minVocalFrequency = 50;
            var maxVocalFrequency = 8000;
            var sampleRate = AudioSettings.outputSampleRate;
            var frequencyResolution = sampleRate / 2 / size;

            //var audioSource = (output as InbuiltAudioOutput).AudioSource;
            var spectrumData = new float[size];                    
            audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

            var indices = Enumerable.Range(0, size - 1).ToList();
            var minVocalFrequencyIndex = indices.Min(x => (Mathf.Abs(x * frequencyResolution - minVocalFrequency), x)).x;
            var maxVocalFrequencyIndex = indices.Min(x => (Mathf.Abs(x * frequencyResolution - maxVocalFrequency), x)).x;
            var indexRange = maxVocalFrequencyIndex - minVocalFrequency;

            spectrumData = spectrumData.Select(x => 1000 * x)
                .ToList()
                .GetRange(minVocalFrequency, indexRange)
                .ToArray();
            view.DisplaySpectrum(spectrumData);
        }
        
        
    }
}