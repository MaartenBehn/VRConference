namespace Network
{
    public enum Packets
    {
        // 1 - 3 Base
        debugMessage = 1,
        
        serverInit = 2, 
        serverUserJoined = 3,
        serverUserLeft = 4,

        featureSettings = 5,

        // 4 - 9 UDP
        clientStartUDP = 6,
        serverUDPConnection = 7,
        clientUDPConnection = 8,

        // Container Packages
        clientContainerPacket = 9,
        
        // 10 Engine
        userVoiceId = 11, 
        
        // 20 Movement
        userFirstPersonPos = 20, 
        userVRPos = 21, 
        
        // 30 FileShare
        userGetListOfLocalFiles = 30,
        userListOfLocalFiles = 31,
        
        userGetFile = 32,
        userFileSyncConfig = 33,
        userGetFilePart= 34,
        userFilePart = 35,
        
        userSyncFailed = 36,
        
        // 40 AudioSpeaker
        audioSpeakerPlaySong = 40,
        
        // 50 FBX Loader
        fbxLoadFile = 50,
        fbxUnloadFile = 51,
        scaleModel = 52,
        
    }

    public enum ContainerType
    {
        all = 1,
        allExceptOrigin = 2,
        list = 3,
        allExceptList = 4,
    }

    public static class State
    {
        public const int BufferSize = 4096;
        public const int MaxClients = 255;
        public const int HeaderSize = 7;
    }
}