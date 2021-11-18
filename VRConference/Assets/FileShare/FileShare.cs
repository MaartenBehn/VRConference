using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private PublicEventStringArray addFilenames;

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

        public void AddFileEntry(byte userId, string name, string path = null)
        {
            bool found = false;
            foreach (FileEntry entry in fileEntries)
            {
                if (entry.fileName == name)
                {
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
        
    }
}