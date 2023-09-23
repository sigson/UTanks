//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Numerics;
//using System.Reflection;
//using System.Runtime.Serialization;
//using System.Text;
//using UTanksClient.Core.Commands;
//using UTanksClient.ECS.ECSCore;
//using UTanksClient.ECS.Components.Battle.Tank;
//using UTanksClient.ECS.Events.Battle;
//using UTanksClient.ECS.Events.Battle.Movement;
//using UTanksClient.ECS.Types;
//using UTanksClient.Library;
//using static UTanksClient.Core.Commands.PacketTools;

//namespace UTanksClient.Core.Protocol
//{
//    class DataDecoder
//    {
//        public bool IsCommandIgnored { get; private set; }

//        private readonly BinaryReader reader;
//        private readonly OptionalMap map;

//        private readonly Dictionary<Type, Func<object>> decodeMethods;

//        private DataDecoder()
//        {
//            decodeMethods = new Dictionary<Type, Func<object>>
//            {
//                { typeof(string), DecodeString },
//                { typeof(Vector3), DecodeVector3 },
//                { typeof(MoveCommand), DecodeMoveCommand },
//                { typeof(Movement), DecodeMovement },
//                { typeof(Type), DecodeType },
//                { typeof(DateTime), DecodeDateTime }
//            };
//        }

//        public DataDecoder(BinaryReader reader) : this()
//        {
//            this.reader = reader;
//        }

//        private DataDecoder(BinaryReader reader, OptionalMap map) : this()
//        {
//            this.reader = reader;
//            this.map = map;
//        }

//        private object DecodePrimitive(Type objType)
//        {
//            return reader.GetType()
//                         .GetMethods()
//                         .Single(method => method.Name == "Read" + objType.Name)
//                         .Invoke(reader, null);
//        }

//        private object DecodeString()
//        {
//            return Encoding.UTF8.GetString(reader.ReadBytes(DecodeLength(reader)));
//        }

//        private int DecodeLength(BinaryReader buf)
//        {
//            byte num1 = buf.ReadByte();
//            if ((num1 & 128) == 0) return num1;
            
//            byte num2 = buf.ReadByte();
//            if ((num1 & 64) == 0) return ((num1 & 63) << 8) + (num2 & byte.MaxValue);
            
//            byte num3 = buf.ReadByte();
//            return ((num1 & 63) << 16) + ((num2 & byte.MaxValue) << 8) + (num3 & byte.MaxValue);
//        }

//        private object DecodeCollection(Type objType, Player player)
//        {
//            int count = DecodeLength(reader);

//            if (objType.IsArray)
//            {
//                Type elementType = objType.GetElementType();
//                Array array = Array.CreateInstance(elementType, count);

//                for (int i = 0; i < count; i++)
//                {
//                    object item = SelectDecode(elementType, player);

//                    if (elementType == typeof(ECSEntity) && item == null)
//                    {
//                        IsCommandIgnored = true;
//                        continue;
//                    }

//                    array.SetValue(item, i);
//                }

//                return array;
//            }

//            object obj = Activator.CreateInstance(objType);
//            Type collectionInnerType = objType.GetGenericArguments()[0];

//            for (int i = 0; i < count; i++)
//            {
//                object item = SelectDecode(collectionInnerType, player);
//                if (collectionInnerType == typeof(ECSEntity) && item == null) continue;

//                objType.GetMethod("Add", new Type[] { collectionInnerType })
//                    .Invoke(obj, new object[] { item });
//            }

//            return obj;
//        }

//        private object DecodeEntity(Player player)
//        {
//            Int64 EntityId = reader.ReadInt64();

//            return player.FindEntityById(EntityId);
//        }

//        private object DecodeDateTime()
//        {
//            return new DateTime(DateTime.UnixEpoch.Ticks + reader.ReadInt64() * 10000);
//        }

//        private object DecodeVector3()
//        {
//            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
//        }

//        private object DecodeCommand(Player player)
//        {
//            Type objType = FindCommandType(reader.ReadByte());

//            return DecodeObject(objType, player);
//        }

//        private object DecodeMoveCommand()
//        {
//            const int WEAPON_ROTATION_SIZE = 2;
//            const int WEAPON_ROTATION_COMPONENT_BITSIZE = WEAPON_ROTATION_SIZE * 8;
//            const float WEAPON_ROTATION_FACTOR = 360f / (1 << WEAPON_ROTATION_COMPONENT_BITSIZE);

//            byte[] bufferEmpty = Array.Empty<byte>();
//            byte[] bufferForWeaponRotation = new byte[WEAPON_ROTATION_SIZE];

//            BitArray bitsEmpty = new(bufferEmpty);
//            BitArray bitsForWeaponRotation = new(bufferForWeaponRotation);

//            bool hasMovement = map.Read();
//            bool hasWeaponRotation = map.Read();
//            bool isDiscrete = map.Read();

//            MoveCommand moveCommand = default;
//            if (isDiscrete)
//            {
//                DiscreteTankControl discreteTankControl = new() { Control = reader.ReadByte() };
//                moveCommand.TankControlHorizontal = discreteTankControl.TurnAxis;
//                moveCommand.TankControlVertical = discreteTankControl.MoveAxis;
//                moveCommand.WeaponRotationControl = discreteTankControl.WeaponControl;
//            }
//            else
//            {
//                moveCommand.TankControlVertical = reader.ReadSingle();
//                moveCommand.TankControlHorizontal = reader.ReadSingle();
//                moveCommand.WeaponRotationControl = reader.ReadSingle();
//            }

//            if (hasMovement)
//                moveCommand.Movement = MovementCodec.Decode(reader);

//            byte[] buffer = hasWeaponRotation ? bufferForWeaponRotation : bufferEmpty;
//            BitArray bits = hasWeaponRotation ? bitsForWeaponRotation : bitsEmpty;
//            int weaponRotationBitLength = 0;

//            reader.Read(buffer, 0, buffer.Length);
//            MovementCodec.CopyBits(buffer, bits);

//            if (hasWeaponRotation)
//                moveCommand.WeaponRotation = new float?(MovementCodec.ReadFloat(bits, ref weaponRotationBitLength, WEAPON_ROTATION_COMPONENT_BITSIZE, WEAPON_ROTATION_FACTOR));

//            if (weaponRotationBitLength != bits.Length)
//                throw new Exception("Move command unpack mismatch");

//            moveCommand.ClientTime = reader.ReadInt32();

//            return moveCommand;
//        }

//        private object DecodeMovement()
//        {
//            return MovementCodec.Decode(reader);
//        }

//        private object DecodeType()
//        {
//            return SerialVersionUIDTools.FindType(reader.ReadInt64());
//        }

//        private object SelectDecode(Type objType, Player player)
//        {
//            if (objType.IsPrimitive || objType == typeof(decimal))
//            {
//                return DecodePrimitive(objType);
//            }

//            if (decodeMethods.TryGetValue(objType, out Func<object> method))
//            {
//                return method();
//            }

//            if (objType.IsEnum)
//            {
//                return Enum.ToObject(objType, reader.ReadByte());
//            }

//            if (objType == typeof(Entity))
//            {
//                return DecodeEntity(player);
//            }

//            // if (typeof(DateTimeOffset).IsAssignableFrom(objType))
//            // {
//            //     long time = reader.ReadInt64();
//            //     return DateTimeOffset.FromUnixTimeMilliseconds(time + player.Connection.DiffToClient);
//            // }

//            if (typeof(IDictionary).IsAssignableFrom(objType))
//            {
//                
//            }
            
//            if (objType.IsArray || (objType.IsGenericType && typeof(ICollection<>).MakeGenericType(objType.GetGenericArguments()[0]).IsAssignableFrom(objType)))
//            {
//                return DecodeCollection(objType, player);
//            }

//            if (typeof(ICommand).IsAssignableFrom(objType))
//            {
//                return DecodeCommand(player);
//            }

//            if (objType.IsAbstract || objType.IsInterface)
//            {
//                objType = (Type)DecodeType();
//                return UnpackData(player, objType);
//            }

//            return DecodeObject(objType, player);
//        }

//        private object DecodeObject(Type objType, Player player)
//        {
//            object obj = FormatterServices.GetUninitializedObject(objType);

//            foreach (PropertyInfo info in GetProtocolProperties(objType))
//            {
//                if (Attribute.IsDefined(info, typeof(OptionalMappedAttribute)))
//                    if (map.Read()) continue;

//                object decoded = SelectDecode(info.PropertyType, player);
//                if (info.PropertyType == typeof(Entity) && decoded == null)
//                {
//                    IsCommandIgnored = true;
//                    continue;
//                }

//                info.SetValue(obj, decoded);
//            }

//            return obj;
//        }

//        private object UnpackData(Player player, Type objType = null)
//        {
//            Int32 mapLength, length;

//            // Magic.
//            byte[] readMagic = reader.ReadBytes(2);
//            if (readMagic.Length < 2)
//                throw new IOException("Unexpected client error.");
//            else if (!readMagic.SequenceEqual(Magic))
//                throw new IOException($"Invalid packet magic: {BitConverter.ToString(readMagic)}");

//            // Length values.
//            mapLength = reader.ReadInt32();
//            length = reader.ReadInt32();

//            // OptionalMap and packet contents.
//            OptionalMap map = new OptionalMap(reader.ReadBytes((int)Math.Ceiling(mapLength / 8.0)), mapLength);

//            using (MemoryStream stream = new MemoryStream(reader.ReadBytes(length)))
//            {
//                BinaryReader buffer = new BigEndianBinaryReader(stream);
//                DataDecoder decoder = new DataDecoder(buffer, map);

//                if (objType != null)
//                {
//                    object obj = decoder.DecodeObject(objType, player);
//                    IsCommandIgnored |= decoder.IsCommandIgnored;
//                    return obj;
//                }

//                // Deserialize objects.
//                List<ICommand> commands = new List<ICommand>();

//                while (buffer.BaseStream.Position != length)
//                {
//                    ICommand command = decoder.SelectDecode(typeof(ICommand), player) as ICommand;
//                    if (decoder.IsCommandIgnored)
//                    {
//                        decoder.IsCommandIgnored = false;
//                        continue;
//                    }

//                    commands.Add(command);
//                }

//                return commands;
//            }
//        }

//        public List<ICommand> DecodeCommands(Player player) => UnpackData(player) as List<ICommand>;
//    }
//}
