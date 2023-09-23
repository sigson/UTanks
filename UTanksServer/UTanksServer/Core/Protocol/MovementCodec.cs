using System;
using System.Collections;
using System.IO;
using System.Numerics;
using UTanksServer.Core.Logging;
using UTanksServer.ECS.Components.Battle;
using UTanksServer.ECS.Components.Battle.Tank;

namespace UTanksServer.Core.Protocol
{
    public class MovementCodec
    {
        private const int MovementSize = 21;
        private const int PositionComponentBitsize = 17;
        private const int OrientationComponentBitsize = 13;
        private const int LinearVelocityComponentBitsize = 13;
        private const int AngularVelocityComponentBitsize = 13;

        private const float PositionFactor = 0.01f;
        private const float VelocityFactor = 0.01f;
        private const float AngularVelocityFactor = 0.005f;
        private const float OrientationPrecision = 2f / (1 << OrientationComponentBitsize);

        public static void Encode(BinaryWriter writer, Movement movement)
        {
            byte[] data = new byte[MovementSize];
            BitArray bits = new BitArray(data);
            
            int position = 0;
            //WriteVector3(bits, ref position, movement.Position, PositionComponentBitsize, PositionFactor);
            //WriteQuaternion(bits, ref position, movement.Orientation, OrientationComponentBitsize, OrientationPrecision);
            //WriteVector3(bits, ref position, movement.Velocity, LinearVelocityComponentBitsize, VelocityFactor);
            //WriteVector3(bits, ref position, movement.AngularVelocity, AngularVelocityComponentBitsize, AngularVelocityFactor);
            
            bits.CopyTo(data, 0);
            writer.Write(data);
            if (position != bits.Length) 
                throw new Exception("Movement pack mismatch");
        }

        public static Movement Decode(BinaryReader reader)
        {
            Movement movement = new Movement();

            byte[] data = reader.ReadBytes(MovementSize);
            BitArray bits = new BitArray(data);

            int position = 0;
            //movement.Position = ReadVector3(bits, ref position, PositionComponentBitsize, PositionFactor);
            //movement.Orientation = ReadQuaternion(bits, ref position, OrientationComponentBitsize, OrientationPrecision);
            //movement.Velocity = ReadVector3(bits, ref position, LinearVelocityComponentBitsize, VelocityFactor);
            //movement.AngularVelocity = ReadVector3(bits, ref position, AngularVelocityComponentBitsize, AngularVelocityFactor);
            
            if (position != bits.Length)
                throw new Exception("Movement unpack mismatch");
            return movement;
        }
        
        private static Vector3 ReadVector3(
            BitArray bits,
            ref int position,
            int size,
            float factor)
        {
            return new Vector3()
            {
                X = ReadFloat(bits, ref position, size, factor),
                Y = ReadFloat(bits, ref position, size, factor),
                Z = ReadFloat(bits, ref position, size, factor)
            };
        }

        private static Quaternion ReadQuaternion(
            BitArray bits,
            ref int position,
            int size,
            float factor)
        {
            Quaternion quaternion = new Quaternion()
            {
                X = ReadFloat(bits, ref position, size, factor),
                Y = ReadFloat(bits, ref position, size, factor),
                Z = ReadFloat(bits, ref position, size, factor)
            };
            quaternion.W = Sqrt((float) (1.0 - ((double) quaternion.X * quaternion.X +
                                                (double) quaternion.Y * quaternion.Y +
                                                (double) quaternion.Z * quaternion.Z)));
            if (double.IsNaN(quaternion.W))
                quaternion.W = 0.0f;
            return quaternion;
        }

        private static float Sqrt(float f)
        {
            return (float) Math.Sqrt(f);
        }

        private static void WriteVector3(
            BitArray bits,
            ref int position,
            Vector3 value,
            int size,
            float factor)
        {
            WriteFloat(bits, ref position, value.X, size, factor);
            WriteFloat(bits, ref position, value.Y, size, factor);
            WriteFloat(bits, ref position, value.Z, size, factor);
        }

        private static void WriteQuaternion(
            BitArray bits,
            ref int position,
            Quaternion value,
            int size,
            float factor)
        {
            int num = (double) value.W < 0.0 ? -1 : 1;
            WriteFloat(bits, ref position, value.X * num, size, factor);
            WriteFloat(bits, ref position, value.Y * num, size, factor);
            WriteFloat(bits, ref position, value.Z * num, size, factor);
        }

        public static void CopyBits(byte[] buffer, BitArray bits)
        {
            for (int index1 = 0; index1 < buffer.Length; ++index1)
            {
                for (int index2 = 0; index2 < 8; ++index2)
                {
                    int index3 = index1 * 8 + index2;
                    bool flag = ((int) buffer[index1] & 1 << index2) != 0;
                    bits.Set(index3, flag);
                }
            }
        }

        public static float ReadFloat(BitArray bits, ref int position, int size, float factor)
        {
            float val = (Read(bits, ref position, size) - (1 << size - 1)) * factor;
            if (IsValidFloat(val))
                return val;
            Logger.Warn($"AbstractMoveCodec.ReadFloat: invalid float: {val}");
            return 0.0f;
        }

        private static bool IsValidFloat(float val)
        {
            return !(float.IsInfinity(val) || float.IsNaN(val));
        }

        public static void WriteFloat(
            BitArray bits,
            ref int position,
            float value,
            int size,
            float factor)
        {
            int offset = 1 << size - 1;
            Write(bits, ref position, size, PrepareValue(value, offset, factor));
        }

        private static int PrepareValue(float val, int offset, float factor)
        {
            int num1 = (int) (val / (double) factor);
            int num2 = 0;
            if (num1 < -offset)
                Logger.Warn($"Value too small {val} offset={offset} factor={factor}");
            else
                num2 = num1 - offset;
            if (num2 >= offset)
            {
                Logger.Warn($"Value too big {val} offset={offset} factor={factor}");
                num2 = offset;
            }

            return num2;
        }

        private static int Read(BitArray bits, ref int position, int bitsCount)
        {
            if (bitsCount > 32)
                throw new Exception($"Cannot read more that 32 bit at once (requested {bitsCount})");
            if (position + bitsCount > bits.Length)
                throw new Exception(
                    $"BitArea is out of data: requested {bitsCount} bits, available:{(bits.Length - position)}");
            int num = 0;
            for (int index = bitsCount - 1; index >= 0; --index)
            {
                if (bits.Get(position))
                    num += 1 << index;
                ++position;
            }

            return num;
        }

        private static void Write(BitArray bits, ref int position, int bitsCount, int value)
        {
            if (bitsCount > 32)
                throw new Exception($"Cannot write more that 32 bit at once (requested {bitsCount})");
            if (position + bitsCount > bits.Length)
                throw new Exception(
                    $"BitArea overflow attempt to write {bitsCount} bits, space avaliable:{(bits.Length - position)}");
            for (int index = bitsCount - 1; index >= 0; --index)
            {
                bool flag = (value & 1 << index) != 0;
                bits.Set(position, flag);
                ++position;
            }
        }
    }
}