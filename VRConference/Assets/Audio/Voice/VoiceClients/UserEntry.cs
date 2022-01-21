using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Audio.Voice.VoiceClients
{
    public class UserEntry : MonoBehaviour
    {
        public event Action<bool> OnIncomingModified;
        public event Action<bool> OnOutgoingModified;

        [SerializeField] TMP_Text idText;
        [SerializeField] Transform barContainer;
        [SerializeField] Transform barTemplate;
        [SerializeField] Toggle speakerToggle;
        [SerializeField] Toggle micToggle;

        public bool IncomingAudio {
            get => speakerToggle.isOn;
            set => speakerToggle.SetIsOnWithoutNotify(!value);
        }

        public bool OutgoingAudio {
            get => micToggle.isOn;
            set => micToggle.SetIsOnWithoutNotify(!value);
        }

        List<Transform> bars = new List<Transform>();

        void Start() {
            speakerToggle.onValueChanged.AddListener(value =>
                OnIncomingModified?.Invoke(value));

            micToggle.onValueChanged.AddListener(value =>
                OnOutgoingModified?.Invoke(value));
        }

        public void SetPeerID(short id) {
            idText.text = id.ToString();
        }

        public void DisplaySpectrum(float[] spectrum) {
            InitBars(spectrum.Length);

            if (spectrum.Length != bars.Count) return;

            for (int i = 0; i < bars.Count; i++)
                bars[i].localScale = new Vector3(1, Mathf.Clamp01(spectrum[i]), 1);
        }

        void InitBars(int count) {
            if (bars.Count == count) return;

            foreach (var bar in bars)
                Destroy(bar.gameObject);
            bars.Clear();

            for (int i = 0; i < count; i++) {
                var instance = Instantiate(barTemplate, barContainer);
                instance.gameObject.SetActive(true);
                bars.Add(instance);
            }
        }
    }
}