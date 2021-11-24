using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utility.Console
{
    public class ConsoleHandler : MonoBehaviour
    {
        public static ConsoleHandler instance;
    
        void Awake () {

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable () {
            Application.logMessageReceived -= HandleLog;
        }

        [Serializable]
        public class ConsoleMessage
        {
            public float startTime;
            public string message;
        }

        public List<ConsoleMessage> messages;
        public float liveTime = 10;
        public PublicEventString newLogMessage;

        void HandleLog (string logString, string stackTrace, LogType type)
        {
            Threader.RunOnMainThread(() =>
            {
                ConsoleMessage message = new ConsoleMessage();
                message.message = logString;
                message.startTime = Time.time;
                messages.Add(message);
                newLogMessage.Raise(logString);
            });
        }

        private void Update()
        {
            while (messages.Count > 0 && messages[0].startTime + liveTime <= Time.time)
            {
                messages.RemoveAt(0);
            }
        }
    }
}
