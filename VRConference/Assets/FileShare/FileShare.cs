using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Network;
using SimpleFileBrowser;
using UnityEngine;
using Utility;

namespace FileShare
{
    
    [Serializable]
    public class FileEntry
    {
        public string fileName;
        public List<byte> userHowHaveTheFile;
        public string localPath;
        public bool local;
    }
    
    [Serializable]
    public class FileSyncConfig
    {
        public FileEntry fileEntry;
        public int partAmmount;
        public int currentPart;
        public int length;
        public byte user;
        public byte[] data;
        public BinaryReader reader;
        public FileStream fileStream;
    }
    
    public class FileShare : MonoBehaviour
    {
        public static FileShare instance;

        [SerializeField] private PublicByte userId;
        
        [SerializeField] private PublicEventString uploadEvent;

        [SerializeField] private bool autoSyncFiles;
        
        public List<FileEntry> fileEntries;

        [SerializeField] private float syncIntervall = 5f;

        [SerializeField] private PublicString savePath;
        
        [SerializeField] private PublicEvent syncFilesEvent;

        [SerializeField] private string[] startUpFiles;

        [SerializeField] private PublicEvent loadingDoneEvent;
        
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
                NetworkController.instance.networkSend.ListOfLocalFilesToAll(GetLocalFiles());
            });

            syncFilesEvent.Register(SyncFiles);

            foreach (string startUpFile in startUpFiles)
            {
                AddFileEntry(userId.value ,startUpFile, Application.dataPath + "/StreamingAssets/StartUpFiles/" + startUpFile +".mp3");
            }

            loadingDoneEvent.Register(SyncFiles);
        }
        
        public void SyncFiles()
        {
            if (savePath.value != "")
            {
                NetworkController.instance.networkSend.GetListOfLocalFiles();
            }
            else
            {
                Debug.Log("No Save Path!");
            }
        }

        public string[] GetLocalFiles()
        {
            List<string> fileNames = new List<string>();
            foreach (FileEntry fileEntry in fileEntries)
            {
                if (fileEntry.local)
                {
                    fileNames.Add(fileEntry.fileName);
                }
            }

            return fileNames.ToArray();
        }

        public void AddFileEntry(byte userId, string name, string path = "")
        {
            bool found = false;
            for (var i = 0; i < fileEntries.Count; i++)
            {
                FileEntry entry = fileEntries[i];
                if (entry.fileName == name)
                {
                    if (path != "")
                    {
                        entry.local = true;
                        entry.localPath = path;
                    }

                    if (!entry.userHowHaveTheFile.Contains(userId))
                    {
                        entry.userHowHaveTheFile.Add(userId);
                    }

                    fileEntries[i] = entry;

                    found = true;
                }
            }

            if (!found)
            {
                FileEntry fileEntry = new FileEntry();
                fileEntry.fileName = name;
                fileEntry.local = path != "";
                fileEntry.localPath = path;
                fileEntry.userHowHaveTheFile = new List<byte>();
                fileEntry.userHowHaveTheFile.Add(userId);
                fileEntries.Add(fileEntry);
            }
        }

        public bool syncingFile;
        [HideInInspector] public FileSyncConfig fileSyncConfig;
        private float lastSyncTime;
        [SerializeField] private int partSize = 2024;
        private void Update()
        {
            if (autoSyncFiles && !syncingFile && !(lastSyncTime + syncIntervall > Time.time))
            {
                lastSyncTime = Time.time;
                foreach (FileEntry fileEntry in fileEntries)
                {
                    if (!fileEntry.local)
                    {
                        SyncFile(fileEntry);
                        break;
                    }
                }
            }

            PrintNetworkSpeed();
        }
        public void SyncFile(FileEntry fileEntry)
        {
            if (fileEntry.local || fileEntry.userHowHaveTheFile.Count == 0) { return; }
            
            syncingFile = true;
            fileSyncConfig = new FileSyncConfig();
            fileSyncConfig.fileEntry = fileEntry;
            
            fileSyncConfig.fileEntry.localPath = savePath.value + fileEntry.fileName;
            fileSyncConfig.fileStream = new FileStream(fileSyncConfig.fileEntry.localPath, FileMode.Create);

            fileSyncConfig.user = fileEntry.userHowHaveTheFile.Contains(0) ? (byte) 0 : fileEntry.userHowHaveTheFile[0];
            NetworkController.instance.networkSend.GetFile(fileSyncConfig);
        }
        public void HandleGetFile(string name, byte fromUser)
        {
            syncingFile = true;

            fileSyncConfig = new FileSyncConfig();
            fileSyncConfig.fileEntry = fileEntries.Find(file => file.fileName == name);
            fileSyncConfig.user = fromUser;
            
            if (File.Exists(fileSyncConfig.fileEntry.localPath))
            {
                fileSyncConfig.length = (int) new FileInfo(fileSyncConfig.fileEntry.localPath).Length;
            }
            else
            {
                Debug.Log("File Not Found " + name);
                syncingFile = false;
                return;
            }

            fileSyncConfig.partAmmount = fileSyncConfig.length / partSize;
            if (fileSyncConfig.length % partSize != 0)
            {
                fileSyncConfig.partAmmount++;
            }

            fileSyncConfig.currentPart = 0;
            fileSyncConfig.reader = new BinaryReader(new FileStream(fileSyncConfig.fileEntry.localPath, FileMode.Open));
            
            fileSyncConfig.data = new byte[fileSyncConfig.length];
            fileSyncConfig.reader.Read(fileSyncConfig.data, 0, (int)fileSyncConfig.length);
            

            NetworkController.instance.networkSend.FileSyncConfig(fileSyncConfig);
        }

        public void HandleFileSyncConfig(string name, byte userId, int length, int parts)
        {
            if (name != fileSyncConfig.fileEntry.fileName || userId != fileSyncConfig.user)
            {
                Debug.Log("FILE: Wrong User or name. Fix this!");
                return;
            }
            
            fileSyncConfig.length = length;
            fileSyncConfig.partAmmount = parts;
            fileSyncConfig.currentPart = 0;
            fileSyncConfig.data = new byte[fileSyncConfig.length];
            
            NetworkController.instance.networkSend.GetFilePart(fileSyncConfig);
        }
        
        public void HandleGetFilePart(string name, byte userId, int part)
        {
            if (name != fileSyncConfig.fileEntry.fileName || userId != fileSyncConfig.user)
            {
                Debug.Log("FILE: Wrong User or name. Fix this!");
                return;
            }
            fileSyncConfig.currentPart = part;
            int offset = partSize * part;
            byte[] data;
            
            if (part >= fileSyncConfig.partAmmount - 1)
            {
                data = new byte[fileSyncConfig.length - offset];
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = fileSyncConfig.data[i + offset];
                }
                NetworkController.instance.networkSend.FilePart(fileSyncConfig, data);
                
                fileSyncConfig.reader.Dispose();
                syncingFile = false;
                fileSyncConfig = null;
                
                Debug.Log("FILE: Done");
            }
            else
            {
                data = new byte[partSize];
                for (int i = 0; i < partSize; i++)
                {
                    data[i] = fileSyncConfig.data[i + offset];
                }
                NetworkController.instance.networkSend.FilePart(fileSyncConfig, data);
            }
        }
        
        public void HandleFilePart(string name, byte userId, int part, byte[] data)
        {
            if (name != fileSyncConfig.fileEntry.fileName || userId != fileSyncConfig.user)
            {
                Debug.Log("FILE: Wrong User or name. Fix this!");
                return;
            }

            int offset = partSize * part;
            for (int i = 0; i < data.Length; i++)
            {
                fileSyncConfig.data[i + offset] = data[i];
            }

            if (part >= fileSyncConfig.partAmmount - 1)
            {
                Debug.Log("FILE: Done");
                fileSyncConfig.fileStream.Write(fileSyncConfig.data, 0, fileSyncConfig.data.Length);
                fileSyncConfig.fileStream.Close();
                fileSyncConfig.fileEntry.local = true;
                syncingFile = false;
                fileSyncConfig = null;
            }
            else
            {
                fileSyncConfig.currentPart++;
                NetworkController.instance.networkSend.GetFilePart(fileSyncConfig);
            }
        }
        
        private float lastSpeedPart;
        private float updateSpeed = 5;
        private float lastSpeedTime;
        public void PrintNetworkSpeed()
        {
            if (!syncingFile || fileSyncConfig == null || lastSpeedTime + updateSpeed >= Time.time)
            {
                return;
            }
            lastSpeedTime = Time.time;
            
            float delta = fileSyncConfig.currentPart - lastSpeedPart;
            lastSpeedPart = fileSyncConfig.currentPart;
            float speed = (delta  * partSize) / updateSpeed;
            float percent = ((float) fileSyncConfig.currentPart / fileSyncConfig.partAmmount) * 100f;
            float time = (fileSyncConfig.length - fileSyncConfig.currentPart * partSize) / speed;
            
            Debug.LogFormat("FILE: Stats: {0} byts/sec {1}% {2} sec remaining.", speed, percent, time);
        }
    }
}