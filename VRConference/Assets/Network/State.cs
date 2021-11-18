namespace Network.Both
{
    public enum Packets
    {
        // 1 - 3 Base
        debugMessage = 1,
        
        serverInit = 2, // user Id, int 
        serverUserJoined = 3,
        serverUserLeft = 9,

        featureSettings = 4,

        // 4 - 9 UDP
        clientStartUDP = 5,
        serverUDPConnection = 6,
        clientUDPConnection = 7,

        // Container Packages
        clientContainerPacket = 8, // byte = ContainerType, bool = use UDP, (byte[] userIDs)
        
        // Engine
        userVoiceId = 11, // byte = VoiceID
        
        // Movement
        userPos = 20, // Vec3
        
        // FileShare
        userGetListOfLocalFiles = 30,
        userListOfLocalFiles = 31,
        
        userGetFile = 32,
        userFile = 33,
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
        public const int HeaderSize = 6;
    }
}