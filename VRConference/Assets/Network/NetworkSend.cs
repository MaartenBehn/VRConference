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
                packet.Write(feature.active);
            }
            
            Send(packet, toUser, false, false);
            Debug.Log(log);
        }
        
        public void UserVoiceID(bool wantRelpy)
        {
            using Packet packet = new Packet((byte) Packets.userVoiceId, network.userId.value);
            packet.Write(network.voiceId.value);
            packet.Write(wantRelpy);
            SendToAllExceptOrigen(packet, false, false);
        }
        
        public void UserPos(float3 pos)
        {
            using Packet packet = new Packet((byte) Packets.userPos, network.userId.value);
            packet.Write(pos);
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
            
            Send(packet, fileSyncConfig.user, true, true);
        }
        
        public void FilePart(FileSyncConfig fileSyncConfig, byte[] data)
        {
            using Packet packet = new Packet((byte) Packets.userFilePart, network.userId.value);
            packet.Write(fileSyncConfig.fileEntry.fileName);
            packet.Write(fileSyncConfig.currentPart);
            packet.Write(data.Length);
            packet.Write(data);
            
            Send(packet, fileSyncConfig.user, true, true);
        }
    }
}