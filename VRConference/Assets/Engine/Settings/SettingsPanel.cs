using System;
using TMPro;
using UnityEngine;
using Utility;

namespace Engine.Settings
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField serverIPField;
        [SerializeField] private TMP_InputField serverPortFieldTCP;
        [SerializeField] private TMP_InputField serverPortFieldUDP;
        [SerializeField] private TMP_InputField clientPortFieldTCP;
        [SerializeField] private TMP_InputField clientPortFieldUDP;
        
        [SerializeField] private TMP_InputField signalServerIPField;
        [SerializeField] private TMP_InputField signalServerPortField;
        [SerializeField] private TMP_Dropdown playerTypeDropdown;

        [SerializeField] private PublicString serverIP;
        
        [SerializeField] private PublicInt serverPortTCP;
        [SerializeField] private PublicInt serverPortUDP;
        [SerializeField] private PublicInt clientPortTCP;
        [SerializeField] private PublicInt clientPortUDP;
        
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
            serverPortFieldTCP.text = serverPortTCP.value.ToString();
            serverPortFieldUDP.text = serverPortUDP.value.ToString();
            clientPortFieldTCP.text = clientPortTCP.value.ToString();
            clientPortFieldUDP.text = clientPortUDP.value.ToString();
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
            serverPortTCP.value = Int32.Parse(serverPortFieldTCP.text);
            serverPortUDP.value = Int32.Parse(serverPortFieldUDP.text);
            clientPortTCP.value = Int32.Parse(clientPortFieldTCP.text);
            clientPortUDP.value = Int32.Parse(clientPortFieldUDP.text);
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