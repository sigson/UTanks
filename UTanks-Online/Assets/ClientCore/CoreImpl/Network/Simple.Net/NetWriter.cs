using System;
using System.Collections.Generic;
using System.Text;
using UTanksClient.ECS.Components.Battle;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.ECS.Types.Battle.AtomicType;

namespace UTanksClient.Network.Simple.Net {
    public class NetWriter : NetBuffer {
        public NetWriter(long hashCode)
            => this.hashCode = hashCode;
        
        public void Push(bool value)
            => buffer.AddRange(BitConverter.GetBytes(value));

        public void Push(byte value)
            => buffer.Add(value);

        public void Push(long value)
            => buffer.AddRange(BitConverter.GetBytes(value));
        
        public void Push(double value)
            => buffer.AddRange(BitConverter.GetBytes(value));

        public void Push(float value)
            => buffer.AddRange(BitConverter.GetBytes(value));

        public void Push(string value) {
            if (value == null)
                value = string.Empty;
            
            byte[] content = Encoding.UTF8.GetBytes(value);
            Push((long)content.Length);
            buffer.AddRange(content);
        }

        public void Push(Vector3S value)
        {
            if (value == null)
                value = new Vector3S();

            buffer.AddRange(BitConverter.GetBytes(value.x));
            buffer.AddRange(BitConverter.GetBytes(value.y));
            buffer.AddRange(BitConverter.GetBytes(value.z));
        }

        public void Push(QuaternionS value)
        {
            if (value == null)
                value = new QuaternionS();

            buffer.AddRange(BitConverter.GetBytes(value.x));
            buffer.AddRange(BitConverter.GetBytes(value.y));
            buffer.AddRange(BitConverter.GetBytes(value.z));
            buffer.AddRange(BitConverter.GetBytes(value.w));
        }

        public void Push(WorldPoint value)
        {
            if (value == null)
                value = new WorldPoint();

            buffer.AddRange(BitConverter.GetBytes(value.Position.x));
            buffer.AddRange(BitConverter.GetBytes(value.Position.y));
            buffer.AddRange(BitConverter.GetBytes(value.Position.z));
            buffer.AddRange(BitConverter.GetBytes(value.Rotation.x));
            buffer.AddRange(BitConverter.GetBytes(value.Rotation.y));
            buffer.AddRange(BitConverter.GetBytes(value.Rotation.z));
        }

        public void Push(ICreatureComponent value)
        {
            Push(value.CreatureComponentID);
            Push(value.WorldPositon);
        }

        public void Push<T>(IList<T> listValue, Action<T> valueEncoder)
        {
            Push((long)listValue.Count);
            foreach(T elem in listValue)
            {
                valueEncoder(elem);
            }
        }

        public void Push<T, T1>(IDictionary<T, T1> dictionaryValue, Action<T> keyEncoder, Action<T1> valueEncoder)
        {
            Push((long)dictionaryValue.Count);
            foreach (var elem in dictionaryValue)
            {
                keyEncoder(elem.Key);
                valueEncoder(elem.Value);
            }
        }
    }
}