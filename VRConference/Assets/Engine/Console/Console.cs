using System;
using UnityEngine;
using Utility.Console;

namespace Engine.Console
{
    public class Console : MonoBehaviour
    {
        [SerializeField] private GameObject messagePreFab;
        [SerializeField] private Transform content;

        private int id;
        void OnEnable () {
            
            foreach (ConsoleHandler.ConsoleMessage message in ConsoleHandler.instance.messages)
            {
                Spawn(message.message, message.startTime);
            }

            id = ConsoleHandler.instance.newLogMessage.Register(message =>
            {
                Spawn(message, Time.time);
            });
        }

        void OnDisable () {
            ConsoleHandler.instance.newLogMessage.Unregister(id);
        }

        void Spawn (String logString, float startTime)
        {
            ConsoleMessage message = Instantiate(messagePreFab, content).GetComponent<ConsoleMessage>();
            message.text.text = logString;
            message.startTime = startTime;
            message.liveTime = ConsoleHandler.instance.liveTime;
        }
    }
}