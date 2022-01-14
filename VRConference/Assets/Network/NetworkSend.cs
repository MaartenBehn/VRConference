using System;
using FileShare;
using Network.Both;
using Unity.Mathematics;
using UnityEngine;

namespace Network
{
    public class NetworkSend
    {
        private readonly NetworkController network;

        public NetworkSend(NetworkController network)
        {
            this.network = network;
        }

        private void Send(Packet packet, byte userID, bool useUDP, bool async)
        {
            if (network.isServer.value)
            {
                network.server.Send(userID, packet, useUDP, async);
            }
            else
            {
                SendToList(packet, new[] {userID}, useUDP, async);
            }
        }

        private void SendToAll(Packet packet, bool userUDP, bool async)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAll(packet, userUDP, async);
            }
            else
            {
                network.client.clientSend.ContainerToAll(packet, userUDP, async);
            }
        }
        
        private void SendToAllExceptOrigen(Packet packet, bool userUDP, bool async)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAllExceptOrigen(packet, userUDP, async);
            }
            else
            {
                network.client.clientSend.ContainerToAllExceptOrigin(packet, userUDP, async);
            }
        }
        
        private void SendToList(Packet packet, byte[] userIDs, bool userUDP, bool async)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToList(packet, userIDs, userUDP, async);
            }
            else
            {
                network.client.clientSend.ContainerToList(packet, userIDs,  userUDP, async);
            }
        }
        
        private void SendToAllExceptList(Packet packet, byte[] userIDs, bool userUDP, bool async)
        {
            if (network.isServer.value)
            {
                network.server.serverSend.SendToAllExceptList(packet, userIDs, userUDP, async);
            }
            else
            {
                network.client.clientSend.ContainerToAllExceptList(packet, userIDs, userUDP, async);
            }
        }
        
        public void FeatureSettings(byte toUser, bool getResponse)
        {
            String log = "NETWORK: Sending Feature Settings :\n";
            log += "User " + toUser + "\n";
            
            using Packet packet = new Packet((byte) Packets.featureSettings, network.userId.value);
            packet.Write(getResponse);

            packet.Write(network.featureSettings.features.Length);
            foreach (var feature in network.featureSettings.features)
            {
                log += feature.name + " " + feature.active + "\n";
                packet.Write(feature.name);
                packet.Write(feature.featureState.value == (int) FeatureState.online);
            }
            
            packet.PrepareForSend(false);
            if (network.isServer.value)
            {
                network.server.tcpServer.SendData(toUser, packet.ToArray(), packet.Length());
            }
            else
            {
                network.client.tcpClient.SendData(packet.ToArray(), packet.Length());
            }
            Debug.Log(log);
        }
        
        public void UserVoiceID(bool wantRelpy)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, network.userId.value);
            packet.Write(network.voiceId.value);
            packet.Write(wantRelpy);
            SendToAllExceptOrigen(packet, false, false);
        }
        
        public void UserFirstPersonPos(Transform t)
        {
            using Packet packet = new Packet((byte) Packets.userFirstPersonPos, network.userId.value);
            packet.Write(t.position);
            packet.Write(t.rotation);
            SendToAllExceptOrigen(packet, true, false);
        }
        
        public void UserVRPos(Transform head, Transform hand1, Transform hand2)
        {
            using Packet packet = new Packet((byte) Packets.userVRPos, network.userId.value);
            packet.Write(head.position);
            packet.Write(head.rotation);
            
            packet.Write(hand1.position);
            packet.Write(hand1.rotation);
            packet.Write(hand2.position);
            packet.Write(hand2.rotation);
            SendToAllExceptOrigen(packet, true, false);
        }

        public void GetListOfLocalFiles()
        {
            using Packet packet = new Packet((byte) Packets.userGetListOfLocalFiles, network.userId.value);
            SendToAllExceptOrigen(packet, false, false);
            
            Debug.Log("NETWORK: GetListOfLocalFiles");
        }
        
        public void ListOfLocalFiles(byte toUser, string[] fileNames)
        {
            using Packet packet = new Packet((byte) Packets.userListOfLocalFiles, network.userId.value);
            
            packet.Write(fileNames.Length);
            foreach (string filename in fileNames)
            {
                packet.Write(filename);
            }
            
            Send(packet, toUser,false, false);
            
            Debug.Log("NETWORK: ListOfLocalFiles");
        }
        
        public void ListOfLocalFilesToAll(string[] fileNames)
        {
            using Packet packet = new Packet((byte) Packets.userListOfLocalFiles, network.userId.value);
            
            packet.Write(fileNames.Length);
            foreach (string filename in fileNames)
            {
                packet.Write(filename);
            }
            
            SendToAllExceptOrigen(packet,false, false);
            
            Debug.Log("NETWORK: ListOfLocalFiles");
        }

        public void GetFile(FileSyncConfig fileSyncConfig)
        {
            using Packet packet = new Packet((byte) Packets.userGetFile, network.userId.value);
            packet.Write(fileSyncConfig.fileEntry.fileName);
            Send(packet, fileSyncConfig.user, false, false);
            
            Debug.Log("NETWORK: GetFile " + fileSyncConfig.fileEntry.fileName);
        }
        
        public void FileSyncConfig(FileSyncConfig fileSyncConfig)
        {
            using Packet packet = new Packet((byte) Packets.userFileSyncConfig, network.userId.value);
            packet.Write(fileSyncConfig.fileEntry.fileName);
            packet.Write(fileSyncConfig.length);
            packet.Write(fileSyncConfig.partAmmount);
            Send(packet, fileSyncConfig.user, false, false);
            
            Debug.Log("NETWORK: File Sync Config " + fileSyncConfig.fileEntry.fileName);
        }
        
        public void GetFilePart(FileSyncConfig fileSyncConfig)
        {
            using Packet packet = new Packet((byte) Packets.userGetFilePart, network.userId.value);
            packet.Write(fileSyncConfig.fileEntry.fileName);
            packet.Write(fileSyncConfig.currentPart);
            
            Send(packet, fileSyncConfig.user, false, true);
        }
        
        public void FilePart(FileSyncConfig fileSyncConfig, byte[] data)
        {
            using Packet packet = new Packet((byte) Packets.userFilePart, network.userId.value);
            packet.Write(fileSyncConfig.fileEntry.fileName);
            packet.Write(fileSyncConfig.currentPart);
            packet.Write(data.Length);
            packet.Write(data);
            
            Send(packet, fileSyncConfig.user, false, true);
        }
        
        public void SyncFailed(byte toUser)
        {
            using Packet packet = new Packet((byte) Packets.userSyncFailed, network.userId.value);
            Send(packet, toUser, false, false);
        }
        
        public void SpeakerPlaySong(int id, string name)
        {
            using Packet packet = new Packet((byte) Packets.audioSpeakerPlaySong, network.userId.value);
            packet.Write(id);
            packet.Write(name);
            
            SendToAllExceptOrigen(packet, false, false);
        }
    }
}