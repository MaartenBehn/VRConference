using System;

using Adrenak.UniMic;

namespace Adrenak.UniVoice.InbuiltImplementations {
    /// <summary>
    /// An <see cref="IAudioInput"/> implementation based on UniMic.
    /// For more on UniMic, visit https://www.github.com/adrenak/unimic
    /// </summary>
    public class UniMicAudioInput : IAudioInput {
        public event Action<int, float[]> OnSegmentReady;

        public int Frequency
        {
            get => mic.Frequency;
            set => mic.Frequency = value;
        }
        
        public Delegate[] getInvationList()
        {
            return OnSegmentReady.GetInvocationList();
        }

        public int ChannelCount => 
        mic.AudioClip == null ? 0 : mic.AudioClip.channels;

        public int SegmentRate => 1000 / mic.SampleDurationMS;

        public readonly Mic mic;

        public UniMicAudioInput(Mic mic) {
            this.mic = mic;
            mic.OnSampleReady += Mic_OnSampleReady;
        }

        public void Mic_OnSampleReady(int segmentIndex, float[] samples) {
            OnSegmentReady?.Invoke(segmentIndex, samples);
        }

        public void Dispose() {
            mic.OnSampleReady -= Mic_OnSampleReady;
        }
    }
}
