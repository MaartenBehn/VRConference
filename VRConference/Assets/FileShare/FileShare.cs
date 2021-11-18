using System;
using System.Collections.Generic;
using System.Linq;
using Network;
using UnityEngine;
using Utility;

namespace FileShare
{
    public class FileShare : MonoBehaviour
    {
        public static FileShare instance;
        
        [Serializable]
        public struct FileEntry
        {
            public string fileName;
            public List<byte> userHowHaveTheFile;
            public string localPath;
        }

        [SerializeField] private PublicByte userId;
        
        [SerializeField] private PublicEventString uploadEvent;
        [SerializeField] private PublicEvent syncEvent;

        [SerializeField] private bool autoSyncFiles;
        
        public List<FileEntry> fileEntries;

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
            
            uploadEvent.Register((string path) =>
            {
                var parts = path.Split(new char[] {'/','\\'});
                AddFileEntry(userId.value, parts.Last(), path);
            });

            
        }

        public void AddFileEntry(byte userId, string name, string path = "")
        {
            bool found = false;
            for (var i = 0; i < fileEntries.Count; i++)
            {
                FileEntry entry = fileEntries[i];
                if (entry.fileName == name)
                {
                    if (path != null)
                    {
                        entry.localPath = path;
                    }

                    if (!entry.userHowHaveTheFile.Contains(userId))
                    {
                        entry.userHowHaveTheFile.Add(userId);
                    }

                    found = true;
                }
            }

            if (!found)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.fileName = name;
                fileEntry.localPath = path;
                fileEntry.userHowHaveTheFile = new List<byte>();
                fileEntry.userHowHaveTheFile.Add(userId);
                fileEntries.Add(fileEntry);
            }
        }

        public void SyncFile(FileEntry fileEntry)
        {
            if (fileEntry.localPath != "" || fileEntry.userHowHaveTheFile.Count == 0)
            {
                return;
            }

            if (fileEntry.userHowHaveTheFile.Contains(0))
            {
                NetworkController.instance.networkSend
                    .GetFile(fileEntry.fileName, 0);
            }
            else
            {
                NetworkController.instance.networkSend
                    .GetFile(fileEntry.fileName, fileEntry.userHowHaveTheFile[0]);
            }
        }

        private void Update()
        {
            if (!autoSyncFiles)
            {
                return;
            }
            
            foreach (FileEntry fileEntry in fileEntries)
            {
                if (fileEntry.localPath == "")
                {
                    SyncFile(fileEntry);
                    break;
                }
            }
        }
    }
}