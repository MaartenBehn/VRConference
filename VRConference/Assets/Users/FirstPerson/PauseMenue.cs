using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
using Voice;

namespace Users.FirstPerson
{
    public class PauseMenue : MonoBehaviour
    {
    
        [SerializeField] private PublicBool pause;
        [SerializeField] private float lockDelay = 1;
        private float lastLockTime;

        [SerializeField] private GameObject pauseMeune;
        [SerializeField] private PublicEvent doneEvent;

        [SerializeField] private TMP_Dropdown micDropdown;
        [SerializeField] private Slider volumeSlider;

        private void Awake()
        {
            doneEvent.Register(() =>
            {
                Set(true);
            });

            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 1;
        }

        void Update()
        {
            if (lastLockTime + lockDelay < Time.time && Input.GetKey(KeyCode.Escape))
            {
                lastLockTime = Time.time;
                Set(!pause.value);
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

        public void Set(bool value)
        {
            pause.value = value;
            Cursor.lockState = !pause.value ? CursorLockMode.Locked : CursorLockMode.None;
            pauseMeune.SetActive(pause.value);

            if (value)
            {
                volumeSlider.value = AudioListener.volume;
            }
        }

        public void SetMic()
        {
            if (PlayerAudio.instance == null || PlayerAudio.instance.mic == null)
            {
                return;
            }
            
            if (PlayerAudio.instance.mic.CurrentDeviceIndex != micDropdown.value)
            {
                PlayerAudio.instance.mic.ChangeDevice(micDropdown.value);
            }
        }
        
        public void SetVolume()
        {
            AudioListener.volume = volumeSlider.value;
        }
    }
}
