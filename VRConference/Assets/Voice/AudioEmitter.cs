using System;
using Adrenak.UniVoice;
using Adrenak.UniVoice.InbuiltImplementations;
using Unity.Mathematics;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Voice
{
    public class AudioEmitter : MonoBehaviour
    {
        public ChatroomAgent agent;
        [SerializeField] private PublicString signalServerIP;
        [SerializeField] private PublicInt signalServerPort;
        private AudioSource audioSource;
        private AudioEmitterInput audioInput;
        private void OnEnable()
        {
            audioSource = GetComponent<AudioSource>();
            
            string uri = "ws://" + signalServerIP.value + ":" + signalServerPort.value;
            agent = new InbuiltChatroomAgentFactory(uri).Create();
            
            agent.MuteOthers = true;

            audioInput = new AudioEmitterInput();
            
            Delegate[] list = ((UniMicAudioInput) agent.AudioInput).getInvationList();
            foreach (Delegate invaoction in list)
            {
                Action<int, float[]> realSubscriber = (Action<int, float[]>)invaoction;
                audioInput.OnSegmentReady += realSubscriber;
            }
            ((UniMicAudioInput) agent.AudioInput).Dispose();

            agent.AudioInput = audioInput;
            
            agent.AudioInput.Frequency = audioSource.clip.frequency;

            agent.Network.JoinChatroom("VRConference");
        }

        private int i = 0;
        private float nextActionTime = 0f;
        private float period = 0.1f;
        private void FixedUpdate()
        {
            var data = new float[audioSource.clip.frequency];
            audioSource.clip.GetData(data, audioSource.timeSamples);
            ((AudioEmitterInput) agent.AudioInput).OnSampleReady(i, data);
            i++;
        }

        private void OnDisable()
        {
            agent.Network.LeaveChatroom();
        }
    }
}