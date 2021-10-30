using System.Collections.Generic;
using System.Linq;
using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using Adrenak.UniVoice.Samples;
using UnityEngine;
using Utility;

namespace Voice.VoiceClients
{
    // Used code from https://github.com/adrenak/univoice sample
    public class VoiceClientsPanel : MonoBehaviour
    {
        ChatroomAgent agent;
        Dictionary<short, PeerView> peerViews = new Dictionary<short, PeerView>();
        public Transform peerViewContainer;
        public PeerView peerViewPreFab;

        [SerializeField] private PublicEventByte userJoined;
        [SerializeField] private PublicEventByte userLeft;

        private void Awake()
        {
            agent = VoiceConnection.instance.agent;

            userJoined.Register((id) => { UpdateList(); });
            userLeft.Register((id) => { UpdateList(); });

            UpdateList();
        }

        void UpdateList()
        {
            for (int i = 0; i < peerViewContainer.transform.childCount; i++)
            {
                Destroy(peerViewContainer.transform.GetChild(i).gameObject);
            }
            peerViews.Clear();
            
            PeerView view = Instantiate(peerViewPreFab, peerViewContainer);
            view.IncomingAudio = !agent.MuteOthers;
            view.OutgoingAudio = !agent.MuteSelf;

            view.OnIncomingModified += value =>
                agent.MuteOthers = !value;

            view.OnOutgoingModified += value =>
                agent.MuteSelf = !value;

            view.SetPeerID(agent.Network.OwnID);
            peerViews.Add(agent.Network.OwnID, view);

            foreach (var agentPeerOutput in agent.PeerOutputs)
            {
                short id = agentPeerOutput.Key;
                view = Instantiate(peerViewPreFab, peerViewContainer);
                view.IncomingAudio = !agent.PeerSettings[id].muteThem;
                view.OutgoingAudio = !agent.PeerSettings[id].muteSelf;

                view.OnIncomingModified += value =>
                    agent.PeerSettings[id].muteThem = !value;

                view.OnOutgoingModified += value =>
                    agent.PeerSettings[id].muteSelf = !value;

                view.SetPeerID(id);
                peerViews.Add(id, view);
            }
        }

        private void Update()
        {
            foreach (KeyValuePair<short,PeerView> keyValuePair in peerViews)
            {
                PeerView view = keyValuePair.Value;
                short key = keyValuePair.Key;
                AudioSource audioSource = null;
                
                if (key == agent.Network.OwnID)
                {
                   
                }
                else
                {
                    audioSource = (agent.PeerOutputs[key] 
                        as InbuiltAudioOutput)?.AudioSource;
                    updateSpectrum(view, audioSource);
                }
            }
        }

        void updateSpectrum(PeerView view, AudioSource audioSource)
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