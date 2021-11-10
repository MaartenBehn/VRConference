using System.Net;
using System.Net.Sockets;
using Network.Both;

namespace Network.Server
{
    public class ServerClient
    {
        public readonly byte id;
        public TcpClient socket;
        public NetworkStream stream;
        public byte[] receiveBuffer;
        public string ip;
        public IPEndPoint endPoint;
        public NetworkState state;
        

        public ServerClient(byte id, TcpClient socket)
        {
            this.id = id;
            this.socket = socket;
            state = NetworkState.notConnected;
        }
    }
}