using System;
using Engine.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

namespace Menus
{
    public class StartUIController : MonoBehaviour
    {
        public static StartUIController instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            SetVisable(false);
        }

        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject settingsPanel;
        
        [SerializeField] private TMP_InputField serverIPField;
        [SerializeField] private TMP_InputField serverPortFieldTCP;
        [SerializeField] private TMP_InputField serverPortFieldUDP;
        [SerializeField] private TMP_InputField clientPortFieldTCP;
        [SerializeField] private TMP_InputField clientPortFieldUDP;

        [SerializeField] private TMP_InputField signalServerIPField;
        [SerializeField] private TMP_InputField signalServerPortField;
        [SerializeField] private Toggle playerTypeToggle;
        [SerializeField] private TMP_InputField savePathField;

        [SerializeField] private PublicString serverIP;

        [SerializeField] private PublicInt serverPortTCP;
        [SerializeField] private PublicInt serverPortUDP;
        [SerializeField] private PublicInt clientPortTCP;
        [SerializeField] private PublicInt clientPortUDP;

        [SerializeField] private PublicString signalServerIP;
        [SerializeField] private PublicInt siganlServerPort;
        [SerializeField] private PublicBool isVR;
        [SerializeField] private PublicString savePath;

        [SerializeField] private FeatureSettings featureSettings;

        private void OnEnable()
        {
            SetVisable(false);
            Set();
            SetSavePath();
        }

        private void OnDisable()
        {
            Get();
        }

        public void SetVisable(bool visable)
        {
            mainPanel.SetActive(!visable);
            settingsPanel.SetActive(visable);
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
            savePathField.text = savePath.value;
            
            playerTypeToggle.isOn = isVR.value;
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
            savePath.value = savePathField.text;

            isVR.value = false;

            var vr = featureSettings.Get("VR");
            var firstPerson = featureSettings.Get("FirstPerson");

            if (playerTypeToggle.isOn)
            {
                firstPerson.active = false;
                vr.active = true;
                isVR.value = true;
            }
            else
            {
                firstPerson.active = true;
                vr.active = false;
                isVR.value = false;
            }

            featureSettings.Set("VR", vr);
            featureSettings.Set("FirstPerson", firstPerson);
        }

        public void SetSavePath()
        {
            savePathField.text = Application.dataPath +"/";
        }
        
        public void StopProgramm()
        {
            Application.Quit();
        }
    }
}