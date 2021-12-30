using System;
using System.Collections.Generic;
using Engine.AudioSpeaker;
using Engine.User;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;
using Utility;

namespace Network
{
    public class NetworkHandle
    {
        private readonly NetworkController network;

        public NetworkHandle(NetworkController network)
        {
            this.network = network;
            
            packetHandlers = new Dictionary<byte, PacketHandler>()
            {
                { (byte)Packets.featureSettings, FeatureSettings },
                { (byte)Packets.userVoiceId, UserVoiceID },
                { (byte)Packets.userFirstPersonPos, UserFirstPersonPos },
                { (byte)Packets.userVRPos, UserVRPos },
                
                { (byte)Packets.userGetListOfLocalFiles, GetListOfLocalFiles },
                { (byte)Packets.userListOfLocalFiles, ListOfLocalFiles },
                { (byte)Packets.userGetFile, GetFile },
                { (byte)Packets.userFileSyncConfig, FileSyncConfig },
                { (byte)Packets.userGetFilePart, GetFilePart },
                { (byte)Packets.userFilePart, FilePart },
                
                { (byte)Packets.audioSpeakerPlaySong, SpeakerPlaySong },
            };
        }
        
        public delegate void PacketHandler(byte userID, Packet packet);
        public Dictionary<byte, PacketHandler> packetHandlers;
        
        public void FeatureSettings(byte userID, Packet packet)
        {
            String log = "NETWORK: Received Feature Settings from "+ userID +":\n";
            
            User user = UserController.instance.users[userID];

            bool needToReply = packet.ReadBool();
            
            int lenght = packet.ReadInt32();
            for (int i = 0; i < lenght; i++)
            {
                String name = packet.ReadString();
                bool value = packet.ReadBool();
                
                log += name + " " + value + "\n";
                user.features[name] = value;
            }
            
            if (needToReply)
            {
                network.networkSend.FeatureSettings(userID, false);
            }
            
            if (network.networkFeatureState.value != (int) FeatureState.online)
            {
                bool allLoaded = true;
                foreach (User otherUser in UserController.instance.users.Values)
                {
                    if (!otherUser.features.ContainsKey("Network"))
                    {
                        allLoaded = false;
                    }
                }

                if (allLoaded)
                {
                    Debug.Log("Network: online");
                    network.networkFeatureState.value = (int) FeatureState.online;
                }
            }
        }
        
        public void UserVoiceID(byte userID, Packet packet)
        {
            byte vID = packet.ReadByte();
            bool reply = packet.ReadBool();

            User user = UserController.instance.users[userID];
            if (user == null)
            {
                Debug.Log("NETWORK: User not existing");
                return;
            }
            
            user.voiceId = vID;
            
            if (reply)
            {
                network.networkSend.UserVoiceID(false);
            }
            
            Debug.LogFormat("NETWORK: User: {0} VoiceID: {1}", userID, vID);
        }

        public void UserFirstPersonPos(byte userID, Packet packet)
        {
            Threader.RunOnMainThread(() =>
            {
                float3 pos = packet.ReadFloat3();
                Quaternion direction = packet.ReadQuaternion();
                
                UserController.instance.users[userID].SetPosition(pos, direction);
            });
        }
        
        public void UserVRPos(byte userID, Packet packet)
        {
            Threader.RunOnMainThread(() =>
            {
                float3 pos = packet.ReadFloat3();
                Quaternion direction = packet.ReadQuaternion();
                
                float3 hand1Pos = packet.ReadFloat3();
                Quaternion hand1Direction = packet.ReadQuaternion();
                float3 hand2Pos = packet.ReadFloat3();
                Quaternion hand2Direction = packet.ReadQuaternion();
                
                UserController.instance.users[userID].SetPosition(pos, direction);
            });
        }

        public void GetListOfLocalFiles(byte userID, Packet packet)
        {
            List<string> fileNames = new List<string>();
            var fileEntries = FileShare.FileShare.instance.fileEntries;

            foreach (FileShare.FileEntry fileEntry in fileEntries)
            {
                if (fileEntry.local)
                {
                    fileNames.Add(fileEntry.fileName);
                }
            }
            
            network.networkSend.ListOfLocalFiles(userID, fileNames.ToArray());
        }
        
        public void ListOfLocalFiles(byte userID, Packet packet)
        {
            int length = packet.ReadInt32();

            for (int i = 0; i < length; i++)
            {
                string name = packet.ReadString();
                FileShare.FileShare.instance.AddFileEntry(userID, name);
            }
        }
        
        public void GetFile(byte userID, Packet packet)
        {
            string filename = packet.ReadString();
            
            FileShare.FileShare.instance.HandleGetFile(filename, userID);
            Debug.Log("NETWORK: Get File request. " +filename);
        }
        
        public void FileSyncConfig(byte userID, Packet packet)
        {
            string filename = packet.ReadString();
            int length = packet.ReadInt32();
            int parts = packet.ReadInt32();

            FileShare.FileShare.instance.HandleFileSyncConfig(filename, userID, length, parts);
            Debug.Log("NETWORK: File Sync Config received. " +filename);
        }
        
        public void GetFilePart(byte userID, Packet packet)
        {
            string filename = packet.ReadString();
            int part = packet.ReadInt32();

            FileShare.FileShare.instance.HandleGetFilePart(filename, userID, part);
        }
        
        public void FilePart(byte userID, Packet packet)
        {
            string filename = packet.ReadString();
            int part = packet.ReadInt32();
            int length = packet.ReadInt32();
            byte[] data = packet.ReadBytes(length);

            FileShare.FileShare.instance.HandleFilePart(filename, userID, part, data);
        }
        
        public void SpeakerPlaySong(byte userID, Packet packet)
        {
            int id = packet.ReadInt32();
            string name = packet.ReadString();

            SpeakerController.instance.speakers[id].songFileName = name;
        }
    }
}