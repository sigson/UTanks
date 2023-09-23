using System;
using System.Collections.Generic;
using System.Linq;

namespace UTanksServer.Core.Protocol
{
    public class OptionalMap
    {
        public OptionalMap()
        {
        }

        public OptionalMap(byte[] data, Int32 Length)
        {
            this.data = data.ToList();
            this.Length = Length;
        }

        private List<byte> data = new List<byte>();

        public Int32 Length { get; private set; }
        public Int32 Position { get; private set; }

        public void Add(bool isNull)
        {
            if (Position >= Length)
            {
                data.Add(0);
                Length += 8;
            }
            data[Position / 8] |= (byte)(Convert.ToInt32(isNull) << (7 - Position++ % 8));
        }

        public bool Read()
        {
            if (Position >= Length)
            {
                throw new InvalidOperationException("Read attempt in end of OptionalMap");
            }
            return Convert.ToBoolean((data[Position / 8] >> (7 - Position++ % 8)) & 1);
        }

        public void Reset() => Position = 0;

        public byte[] GetBytes()
        {
            return data.ToArray();
        }
    }
}
