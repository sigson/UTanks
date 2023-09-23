using System;
using System.Collections.Generic;
using System.Text;
using UTanksServer.ECS.Components.Battle.AdditionalLogicComponents;
using UTanksServer.ECS.Types.Battle;
using UTanksServer.ECS.Types.Battle.AtomicType;

namespace UTanksServer.Network.Simple.Net {
    public class NetReader : NetBuffer {
        public int readPos = 0;
        
        public NetReader(byte[] buffer) {
            this.buffer.AddRange(buffer);
            this.hashCode = ReadInt64();
            this.id = ReadInt64();
            this.length = ReadInt64();
            this.packetPosition = ReadInt64();
        }
        
        public bool ReadBool() {
            try {
                return BitConverter.ToBoolean(buffer.ToArray(), readPos++);
            } catch {
                throw new OverflowException("Cannot read Bool!");
            }
        }

        public long ReadInt64() {
            try {
                long result = BitConverter.ToInt64(buffer.ToArray(), readPos);
                readPos += 8;
                return result;
            } catch {
                throw new OverflowException("Cannot read Int64!");
            }
        }

        public byte ReadByte()
        {
            try
            {
                byte result = buffer[readPos];
                readPos += 1;
                return result;
            }
            catch
            {
                throw new OverflowException("Cannot read byte!");
            }
        }

        public double ReadDouble() {
            try {
                double result = BitConverter.ToDouble(buffer.ToArray(), readPos);
                readPos += 8;
                return result;
            } catch {
                throw new OverflowException("Cannot read Double!");
            }
        }

        public float ReadFloat()
        {
            try
            {
                float result = BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                return result;
            }
            catch
            {
                throw new OverflowException("Cannot read float!");
            }
        }

        public string ReadString() {
            int length;
            try { length = Convert.ToInt32(ReadInt64()); }
            catch { throw new OverflowException("Cannot read string! Failed to read Int64 length of string"); }
            try {
                string result = Encoding.UTF8.GetString(buffer.ToArray(), readPos, length);
                readPos += length;
                return result;
            } catch {
                throw new OverflowException($"Cannot read string of length '{length}'! End of buffer!");
            }
        }

        public Vector3S ReadVector3()
        {
            try
            {
                float x = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float y = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float z = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                return new Vector3S() { x = x, y = y, z = z};
            }
            catch
            {
                throw new OverflowException("Cannot read Int64!");
            }
        }

        public QuaternionS ReadQuaternion3()
        {
            try
            {
                float x = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float y = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float z = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float w = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                return new QuaternionS() { x = x, y = y, z = z, w = w };
            }
            catch
            {
                throw new OverflowException("Cannot read Quaternion!");
            }
        }
        public WorldPoint ReadWorldPoint()
        {
            try
            {
                float x = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float y = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float z = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float x1 = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float y1 = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                float z1 = (float)BitConverter.ToSingle(buffer.ToArray(), readPos);
                readPos += 4;
                return new WorldPoint() {  Position = new Vector3S() { x = x, y = y, z = z }, Rotation = new Vector3S() { x = x1, y = y1, z = z1 } };
            }
            catch
            {
                throw new OverflowException("Cannot read WorldPoint!");
            }
        }

        public ICreatureComponent ReadICreatureComponent()
        {
            return new ICreatureComponent() {
                CreatureComponentID = ReadInt64(),
                WorldPositon = ReadWorldPoint()
            };
        }
        public List<T> ReadList<T>(Func<T> valueDecoder)
        {
            try
            {
                int count;
                try { count = Convert.ToInt32(ReadInt64()); }
                catch { throw new OverflowException("Cannot read list! Failed to read Int64 length of string"); }
                List<T> result = new List<T>();
                for(int i = 0; i < count; i++)
                {
                    result.Add(valueDecoder());
                }
                return result;
            }
            catch
            {
                throw new OverflowException("Cannot read List!");
            }
        }

        public Dictionary<T, T1> ReadDictionary<T, T1>(Func<T> keyDecoder, Func<T1> valueDecoder) //decoders and encoders - netreader/writer push/read methods for selected type
        {
            try
            {
                int count;
                try { count = Convert.ToInt32(ReadInt64()); }
                catch { throw new OverflowException("Cannot read dictionary! Failed to read Int64 length of string"); }
                Dictionary<T, T1> result = new Dictionary<T, T1>();
                for (int i = 0; i < count; i++)
                {
                    var key = keyDecoder();
                    var value = valueDecoder();
                    result[key] = value;
                }
                return result;
            }
            catch
            {
                throw new OverflowException("Cannot read Dictionary!");
            }
        }
    }
}