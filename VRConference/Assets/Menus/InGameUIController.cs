using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Utility;
using Voice;

namespace Menus
{
    public class InGameUIController : MonoBehaviour
    {
        [SerializeField] private bool IsVrScript = false;
        [SerializeField] private PublicBool isVR;
        [SerializeField] private PublicBool pause;
        [SerializeField] private float lockDelay = 1;
        private float lastLockTime;
        


        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject inGamePanel;

        [SerializeField] private Toggle micMuteToggle;
        [SerializeField] private Toggle speakerMuteToggle;
        [SerializeField] private TMP_Dropdown micDropdown;

        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider voiceSlider;
        [SerializeField] private Slider effectsSlider;

        [SerializeField] private TMP_Text masterVolPanel;
        [SerializeField] private TMP_Text musicVolPanel;
        [SerializeField] private TMP_Text voiceVolPanel;
        [SerializeField] private TMP_Text effectsVolPanel;

        private void Awake()
        {
            if (isVR.value && !IsVrScript)
            {
                gameObject.active = false;
                return;
            }
            masterSlider.maxValue = maxVol;
            masterSlider.minValue = minVol;
            
            musicSlider.maxValue = maxVol;
            musicSlider.minValue = minVol;
            
            voiceSlider.maxValue = maxVol;
            voiceSlider.minValue = minVol;
            
            effectsSlider.maxValue = maxVol;
            effectsSlider.minValue = minVol;
            
            micMuteToggle.isOn = true;
            SetMic();
            GetVolume();
            SetPause();
        }



        void Update()
        {
            if (!isVR.value)
            {
                if (lastLockTime + lockDelay < Time.time && Input.GetKey(KeyCode.Escape))
                {
                    lastLockTime = Time.time;
                    pause.value = !pause.value;

                    if (pause.value)
                    {
                        SetPause();
                    }
                    else
                    {
                        SetInGame();
                    }
                }
            }
            if (PlayerAudio.instance != null && PlayerAudio.instance.mic != null)
            {
                var options = new List<TMP_Dropdown.OptionData>();
                foreach (var micDevice in PlayerAudio.instance.mic.Devices)
                {
                    var option = new TMP_Dropdown.OptionData();
                    option.text = micDevice;
                    options.Add(option);
                }

                micDropdown.options = options;
            }
        }
        
        public void SetInGame()
        {
            inGamePanel.SetActive(true);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(false);
            
            pause.value = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void SetPause()
        {
            if (!isVR.value)
            {
                inGamePanel.SetActive(false);
                pausePanel.SetActive(true);
                settingsPanel.SetActive(false);


                pause.value = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {

                pausePanel.SetActive(true);
                settingsPanel.SetActive(false);
            }

            if (VoiceConnection.instance != null && VoiceConnection.instance.agent != null)
            {
                micMuteToggle.isOn = VoiceConnection.instance.agent.MuteSelf;
            }
        }
        
        public void SetSettings()
        {
            inGamePanel.SetActive(false);
            pausePanel.SetActive(false);
            settingsPanel.SetActive(true);
        }
        
        public void SetMic()
        {
            if (VoiceConnection.instance != null && VoiceConnection.instance.agent != null)
            {
                VoiceConnection.instance.agent.MuteSelf = micMuteToggle.isOn;
            }

            if (PlayerAudio.instance != null && PlayerAudio.instance.mic != null && PlayerAudio.instance.mic.CurrentDeviceIndex != micDropdown.value)
            {
                PlayerAudio.instance.mic.ChangeDevice(micDropdown.value);
            }
        }

        private float oldVolume;
        public void SetSpeaker()
        {
            float vol;
            audioMixer.GetFloat("masterVol", out vol);
            if (speakerMuteToggle.isOn && Math.Abs(vol +80) > 0.1)
            {
                oldVolume = vol;
                audioMixer.SetFloat("masterVol", -80);
            }else if (!speakerMuteToggle.isOn && Math.Abs(vol +80) < 0.1 && Math.Abs(oldVolume +80) > 0.1)
            {
                audioMixer.SetFloat("masterVol", oldVolume);
                oldVolume = -80;
            }

        }

        private const float minVol = 0.0001f;
        private const float maxVol = 1f;
        
        public float ToVolume(float val)
        {
            return (val * 100) - 80;
        }
        
        public float FromVolume(float val)
        {
            return (val + 80) / 100;
        }
        
        public void GetVolume()
        {
            float vol;
            audioMixer.GetFloat("masterVol", out vol);
            masterVolPanel.text = GetVolString(vol);
            masterSlider.SetValueWithoutNotify(FromVolume(vol));
            
            audioMixer.GetFloat("musicVol", out vol);
            musicVolPanel.text =GetVolString(vol);
            musicSlider.SetValueWithoutNotify(FromVolume(vol));
            
            audioMixer.GetFloat("voiceVol", out vol);
            voiceVolPanel.text = GetVolString(vol);
            voiceSlider.SetValueWithoutNotify(FromVolume(vol));
            
            audioMixer.GetFloat("effectsVol", out vol);
            effectsVolPanel.text = GetVolString(vol);
            effectsSlider.SetValueWithoutNotify(FromVolume(vol));
        }
        
        public void SetVolume()
        {
            if (!speakerMuteToggle.isOn)
            {
                audioMixer.SetFloat("masterVol", ToVolume(masterSlider.value));
            }
            
            audioMixer.SetFloat("musicVol", ToVolume(musicSlider.value));
            audioMixer.SetFloat("voiceVol", ToVolume(voiceSlider.value));
            audioMixer.SetFloat("effectsVol", ToVolume(effectsSlider.value));
            
            float vol;
            audioMixer.GetFloat("masterVol", out vol);
            masterVolPanel.text = GetVolString(vol);

            audioMixer.GetFloat("musicVol", out vol);
            musicVolPanel.text = GetVolString(vol);

            audioMixer.GetFloat("voiceVol", out vol);
            voiceVolPanel.text = GetVolString(vol);

            audioMixer.GetFloat("effectsVol", out vol);
            effectsVolPanel.text = GetVolString(vol);
        }

        string GetVolString(float vol)
        {
            return $"{vol:0} dB";
        }
    }
    
}