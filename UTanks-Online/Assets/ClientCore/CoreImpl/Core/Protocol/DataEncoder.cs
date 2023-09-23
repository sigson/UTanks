//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Numerics;
//using System.Reflection;
//using System.Text;
//using UTanksClient.Core.Commands;
//using UTanksClient.ECSSystem.Base;
//using UTanksClient.ECSSystem.Components.Battle.Tank;
//using UTanksClient.ECSSystem.Events.Battle;
//using UTanksClient.ECSSystem.Events.Battle.Movement;
//using UTanksClient.ECSSystem.Types;
//using UTanksClient.Library;
//using static UTanksClient.Core.Commands.PacketTools;

//namespace UTanksClient.Core.Protocol
//{
//    class DataEncoder
//    {
//        private static byte[] bufferEmpty = Array.Empty<byte>();
//        private static BitArray bitsEmpty = new BitArray(bufferEmpty);
//        private const int WEAPON_ROTATION_SIZE = 2;
//        private static byte[] bufferForWeaponRotation = new byte[WEAPON_ROTATION_SIZE];
//        private static BitArray bitsForWeaponRotation = new BitArray(bufferForWeaponRotation);
//        private const int WEAPON_ROTATION_COMPONENT_BITSIZE = WEAPON_ROTATION_SIZE * 8;
//        private const float WEAPON_ROTATION_FACTOR = 360f / (1 << WEAPON_ROTATION_COMPONENT_BITSIZE);

//        private readonly BinaryWriter writer;
//        private readonly OptionalMap map;

//        private DataEncoder() { }

//        public DataEncoder(BinaryWriter writer)
//        {
//            this.writer = writer;
//        }

//        private DataEncoder(BinaryWriter writer, OptionalMap map)
//        {
//            this.writer = writer;
//            this.map = map;
//        }

//        private void EncodePrimitive(object obj)
//        {
//            writer.GetType()
//                  .GetMethod("Write", new Type[] { obj.GetType() })
//                  .Invoke(writer, new object[] { obj });
//        }

//        private void EncodeLength(int length)
//        {
//            if (length < 128)
//            {
//                writer.Write((byte)(length & 127));
//            }
//            else if (length < 16384)
//            {
//                long num = (length & 16383) + 32768;
//                writer.Write((byte)((num & 65280L) >> 8));
//                writer.Write((byte)(num & 255L));
//            }
//            else
//            {
//                if (length >= 4194304)
//                {
//                    throw new IndexOutOfRangeException("length=" + length);
//                }
//                long num2 = (length & 4194303) + 12582912;
//                writer.Write((byte)((num2 & 16711680L) >> 16));
//                writer.Write((byte)((num2 & 65280L) >> 8));
//                writer.Write((byte)(num2 & 255L));
//            }
//        }

//        private void EncodeString(string str)
//        {
//            byte[] data = Encoding.UTF8.GetBytes(str);
//            EncodeLength(data.Length);
//            writer.Write(data);
//        }

//        private void EncodeCollection(ICollection collection)
//        {
//            writer.Write((byte)collection.Count);

//            foreach (object el in collection)
//            {
//                SelectEncode(el);
//            }
//        }

//        private void EncodeCommand(ICommand command)
//        {
//            Type type = command.GetType();

//            writer.Write(GetCommandCode(type));
//        }

//        private void EncodeEntity(Entity entity)
//        {
//            writer.Write(entity.EntityId);
//        }

//        private void EncodeEnum(Enum @enum)
//        {
//            writer.Write(Convert.ToByte(@enum));
//        }

//        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Удалите неиспользуемые закрытые члены", Justification = "<Ожидание>")]
//        private void EncodeHashSet<T>(HashSet<T> set)
//        {
//            writer.Write((byte)set.Count);

//            foreach (object el in set)
//            {
//                SelectEncode(el);
//            }
//        }

//        private void EncodeDateTime(DateTime time)
//        {
//            writer.Write((time.ToUniversalTime() - DateTime.UnixEpoch).Ticks / 10000);
//        }

//        private void EncodeVector3(Vector3 vector3)
//        {
//            writer.Write(vector3.X);
//            writer.Write(vector3.Y);
//            writer.Write(vector3.Z);
//        }

//        private void EncodeMoveCommand(MoveCommand moveCommand)
//        {
//            bool hasMovement = moveCommand.Movement != null;
//            bool hasWeaponRotation = moveCommand.WeaponRotation != null;
//            bool isDiscrete = moveCommand.IsDiscrete();

//            map.Add(hasMovement);
//            map.Add(hasWeaponRotation);
//            map.Add(isDiscrete);

//            if (isDiscrete)
//            {
//                writer.Write(new DiscreteTankControl()
//                {
//                    MoveAxis = (int)moveCommand.TankControlVertical,
//                    TurnAxis = (int)moveCommand.TankControlHorizontal,
//                    WeaponControl = (int)moveCommand.WeaponRotationControl,
//                }.Control);
//            }
//            else
//            {
//                writer.Write(moveCommand.TankControlVertical);
//                writer.Write(moveCommand.TankControlHorizontal);
//                writer.Write(moveCommand.WeaponRotationControl);
//            }

//            if (hasMovement)
//                MovementCodec.Encode(writer, moveCommand.Movement.Value);

//            if (hasWeaponRotation)
//            {
//                byte[] buffer = bufferForWeaponRotation;
//                BitArray bits = bitsForWeaponRotation;
//                int position = 0;

//                MovementCodec.WriteFloat(bits, ref position, moveCommand.WeaponRotation.Value, WEAPON_ROTATION_COMPONENT_BITSIZE, WEAPON_ROTATION_FACTOR);
//                bits.CopyTo(buffer, 0);
//                writer.Write(buffer);

//                if (position != bits.Length)
//                    throw new Exception("Move command pack mismatch");
//            }

//            writer.Write(moveCommand.ClientTime);
//        }

//        private void EncodeType(Type type)
//        {
//            writer.Write(SerialVersionUIDTools.GetId(type));
//        }
        
//        private void SelectEncode(object obj)
//        {
//            Type objType = obj.GetType();

//            if (objType.IsPrimitive || objType == typeof(decimal))
//            {
//                EncodePrimitive(obj);
//                return;
//            }

//            switch (obj)
//            {
//                case string str:
//                    EncodeString(str);
//                    return;
//                case Entity entity:
//                    EncodeEntity(entity);
//                    return;
//                case IEntityTemplate _:
//                    EncodeType(objType);
//                    return;
//                case ICollection collection:
//                    EncodeCollection(collection);
//                    return;
//                case ICommand command:
//                    EncodeCommand(command);
//                    break;
//                case Enum @enum:
//                    EncodeEnum(@enum);
//                    return;
//                case DateTime date:
//                    EncodeDateTime(date);
//                    return;
//                case Vector3 vector3:
//                    EncodeVector3(vector3);
//                    return;
//                case Movement movement:
//                    MovementCodec.Encode(writer, movement);
//                    return;
//                case MoveCommand moveCommand:
//                    EncodeMoveCommand(moveCommand);
//                    return;
//                case Type type:
//                    EncodeType(type);
//                    return;
//            }

//            if (objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(HashSet<>))
//            {
//                typeof(DataEncoder).GetMethod("EncodeHashSet", BindingFlags.NonPublic | BindingFlags.Instance)
//                                   .MakeGenericMethod(objType.GetGenericArguments()[0])
//                                   .Invoke(this, new object[] { obj });
//                return;
//            }

//            if (Attribute.IsDefined(objType, typeof(SerialVersionUIDAttribute)))
//            {
//                EncodeType(objType);
//            }

//            EncodeObject(obj);
//        }

//        private void EncodeObject(object obj)
//        {
//            foreach (PropertyInfo info in GetProtocolProperties(obj.GetType()))
//            {
//                object value = info.GetValue(obj);

//                if (Attribute.IsDefined(info, typeof(OptionalMappedAttribute)))
//                {
//                    map.Add(value == null);
//                    if (value == null) continue;
//                }
//                else if (value == null)
//                    throw new ArgumentNullException(info.Name, "From " + info.ReflectedType.FullName);

//                SelectEncode(value);
//            }
//        }

//        public void EncodeCommands(IEnumerable<ICommand> commands)
//        {
//            using (MemoryStream memoryStream = new MemoryStream())
//            {
//                BinaryWriter writtenCommands = new BigEndianBinaryWriter(memoryStream);

//                OptionalMap map = new OptionalMap();
//                DataEncoder encoder = new DataEncoder(writtenCommands, map);

//                foreach (ICommand command in commands)
//                {
//                    encoder.SelectEncode(command);
//                }

//                map.Reset();

//                writer.Write(Magic);
//                writer.Write(map.Length);
//                writer.Write((UInt32)writtenCommands.BaseStream.Length);

//                writer.Write(map.GetBytes());

//                writtenCommands.BaseStream.Position = 0;
//                writtenCommands.BaseStream.CopyTo(writer.BaseStream);
//            }
//        }
//    }
//}
