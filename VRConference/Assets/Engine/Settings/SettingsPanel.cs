using System;
using TMPro;
using UnityEngine;
using Utility;

namespace Engine.Settings
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField serverIPField;
        [SerializeField] private TMP_InputField serverPortField;
        [SerializeField] private TMP_InputField clientPortField;
        [SerializeField] private TMP_InputField signalServerIPField;
        [SerializeField] private TMP_InputField signalServerPortField;
        [SerializeField] private TMP_Dropdown playerTypeDropdown;

        [SerializeField] private PublicString serverIP;
        [SerializeField] private PublicInt serverPort;
        [SerializeField] private PublicInt clientPort;
        [SerializeField] private PublicString signalServerIP;
        [SerializeField] private PublicInt siganlServerPort;
        [SerializeField] private PublicBool isFirstPerson;
        [SerializeField] private PublicBool isVR;
        
        private void OnEnable()
        {
            Set();
        }

        private void OnDisable()
        {
            Get();
        }

        private void Set()
        {
            serverIPField.text = serverIP.value;
            serverPortField.text = serverPort.value.ToString();
            clientPortField.text = clientPort.value.ToString();
            signalServerIPField.text = signalServerIP.value;
            signalServerPortField.text = siganlServerPort.value.ToString();

            if (isFirstPerson.value)
            {
                playerTypeDropdown.value = 0;
            }else if (isVR.value)
            {
                playerTypeDropdown.value = 1;
            }
        }

        public void Get()
        {
            serverIP.value = serverIPField.text;
            serverPort.value = Int32.Parse(serverPortField.text);
            clientPort.value = Int32.Parse(clientPortField.text);
            signalServerIP.value = signalServerIPField.text;
            siganlServerPort.value = Int32.Parse(signalServerPortField.text);

            isFirstPerson.value = false;
            isVR.value = false;
            
            switch (playerTypeDropdown.value)
            {
                case 0:
                    isFirstPerson.value = true;
                    break;
                case 1:
                    isVR.value = true;
                    break;
            }
        }
    }
}