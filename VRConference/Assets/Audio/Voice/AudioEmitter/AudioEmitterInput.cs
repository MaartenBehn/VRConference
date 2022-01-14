using System;
using Adrenak.UniVoice;

namespace Voice
{
    public class AudioEmitterInput : IAudioInput
    {
        
        
        public void CopyInvocation(IAudioInput audioInput)
        {
            
        }
        
        public AudioEmitterInput()
        {
            ChannelCount = 1;
            Frequency = 16000;
            SegmentRate = 50;
        }
        
        public void OnSampleReady(int segmentIndex, float[] samples) {
            OnSegmentReady?.Invoke(segmentIndex, samples);
        }
        
        public void Dispose()
        {
            
        }

        public event Action<int, float[]> OnSegmentReady;
        public int Frequency { get; set; }
        public int ChannelCount { get; set; }
        public int SegmentRate { get; set; }
    }
}