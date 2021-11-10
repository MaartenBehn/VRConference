namespace Network.Both
{
    public enum Packets
    {
        // 1 - 3 Base
        debugMessage = 1,
        serverClientId = 2,
        
        featureSettings = 3,

        // 4 - 9 UDP
        clientStartUDP = 4,
        serverUDPConnection = 5,
        clientUDPConnection = 6,

        // Container Packages
        clientContainerPacket = 8, // byte = ContainerType, bool = use UDP, (byte[] userIDs)

        // Engine
        userStatus = 10, // byte 1 = joining, 2 = left
        userVoiceId = 11, // byte = VoiceID
        
        // Movement
        userPos = 20 // Vec3
    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
        connecting = 3,
        disconnecting = 4,
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