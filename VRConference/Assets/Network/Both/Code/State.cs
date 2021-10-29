namespace Network.Both
{
    public enum Packets
    {
        // 1 - 3 Base
        debugMessage = 1,
        serverSettings = 2,  
        clientSettings = 3,

        // 4 - 9 UDP
        serverStartUDP = 4,
        clientUDPConnection = 5,
        serverUDPConnection = 6,
        clientUDPConnectionStatus = 7,
        
        // Engine
        userStatus = 10,
    }

    public enum NetworkState
    {
        notConnected = 1,
        connected = 2,
        connecting = 3,
        disconnecting = 4,
    }

    public static class State
    {
        public const int BufferSize = 4096;
        public const int MaxClients = 255;
        public const int HeaderSize = 6;
    }
}