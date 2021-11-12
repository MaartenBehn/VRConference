using System;
using TMPro;
using UnityEngine;
using Utility;

namespace UI
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_InputField serverIPField;
        [SerializeField] private TMP_InputField serverPortField;
        [SerializeField] private TMP_InputField clientPortField;
        [SerializeField] private TMP_InputField signalServerIPField;
        [SerializeField] private TMP_InputField signalServerPortField;

        [SerializeField] private PublicString serverIP;
        [SerializeField] private PublicInt serverPort;
        [SerializeField] private PublicInt clientPort;
        [SerializeField] private PublicString signalServerIP;
        [SerializeField] private PublicInt siganlServerPort;

        private void Start()
        {
            Set();
        }

        private void Set()
        {
            serverIPField.text = serverIP.value;
            serverPortField.text = serverPort.value.ToString();
            clientPortField.text = clientPort.value.ToString();
            signalServerIPField.text = signalServerIP.value;
            signalServerPortField.text = siganlServerPort.value.ToString();
        }

        public void Get()
        {
            serverIP.value = serverIPField.text;
            serverPort.value = Int32.Parse(serverPortField.text);
            clientPort.value = Int32.Parse(clientPortField.text);
            signalServerIP.value = signalServerIPField.text;
            siganlServerPort.value = Int32.Parse(signalServerPortField.text);
        }
    }
}