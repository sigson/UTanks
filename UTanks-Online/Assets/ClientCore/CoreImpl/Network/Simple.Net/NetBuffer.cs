using System;
using System.Text;
using System.Collections.Generic;

namespace UTanksClient.Network.Simple.Net {
    public class NetBuffer : IDisposable
    {
        public List<byte> buffer { get; set; } = new List<byte>();
        public long hashCode { get; protected set; }
        public long id { get; set; }
        public long length { get; set; }
        public long packetPosition { get; set; }
        public byte[] ToByteArray()
        {
            List<byte> result = new List<byte>();
            long headerSize = 8L * 4L;
            if (ClientNetworkService.instance.Socket.socket.ReceiveBufferSize >= buffer.Count + headerSize)
            {
                result.AddRange(BitConverter.GetBytes(hashCode));
                result.AddRange(BitConverter.GetBytes(Guid.NewGuid().GuidToLong()));
                result.AddRange(BitConverter.GetBytes((long)buffer.Count + headerSize));
                result.AddRange(BitConverter.GetBytes((long)0));
                result.AddRange(buffer);
            }
            else
            {
                List<byte> newBuffer = new List<byte>(buffer);
                int position = 0;
                int packetPosition = 0;
                long guid = Guid.NewGuid().GuidToLong();
                long countOfHeaders = 0;
                var bufferSize = buffer.Count;
                while (bufferSize + headerSize >= ClientNetworkService.instance.Socket.socket.ReceiveBufferSize)
                {
                    countOfHeaders += headerSize;
                    bufferSize -= ClientNetworkService.instance.Socket.socket.ReceiveBufferSize - (int)headerSize;
                }
                if (bufferSize > 0)
                    countOfHeaders += headerSize;
                while (ClientNetworkService.instance.Socket.socket.ReceiveBufferSize < (newBuffer.Count + countOfHeaders) - (position + packetPosition * headerSize))
                {
                    result.AddRange(BitConverter.GetBytes(hashCode));
                    result.AddRange(BitConverter.GetBytes(guid));
                    result.AddRange(BitConverter.GetBytes((long)buffer.Count + countOfHeaders));
                    result.AddRange(BitConverter.GetBytes((long)packetPosition));
                    result.AddRange(buffer.GetRange(position, ClientNetworkService.instance.Socket.socket.ReceiveBufferSize - (int)headerSize));
                    position += ClientNetworkService.instance.Socket.socket.ReceiveBufferSize - (int)headerSize;
                    packetPosition++;
                }
                result.AddRange(BitConverter.GetBytes(hashCode));
                result.AddRange(BitConverter.GetBytes(guid));
                result.AddRange(BitConverter.GetBytes((long)buffer.Count + countOfHeaders));
                result.AddRange(BitConverter.GetBytes((long)packetPosition));
                result.AddRange(buffer.GetRange(position, newBuffer.Count - position));
                position += ClientNetworkService.instance.Socket.socket.ReceiveBufferSize - (int)headerSize;
                packetPosition++;
            }
            return result.ToArray();
        }

        public override string ToString()
        {
            var sb = new StringBuilder("byte[] { ");
            foreach (var b in ToByteArray())
                sb.Append(b + ", ");
            sb.Append("}");
            return sb.ToString();
        }

        public void Dispose()
            => buffer = null;
    }
}