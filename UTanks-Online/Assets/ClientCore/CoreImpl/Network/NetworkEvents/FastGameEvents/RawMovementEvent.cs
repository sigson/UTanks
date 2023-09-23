using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.ECS.Types.Battle;
using UTanksClient.Network.Simple.Net;

namespace UTanksClient.Network.NetworkEvents.FastGameEvents
{
    public struct RawMovementEvent : INetSerializable
    {
        public int packetId;
        public long PlayerEntityId;
        public Vector3S position;
        public Vector3S velocity;
        public Vector3S angularVelocity;
        public QuaternionS rotation;
        public QuaternionS turretRotation;
        public float WeaponRotation { get; set; } //angle
        public float TankMoveControl { get; set; }
        public float TankTurnControl { get; set; }
        public float WeaponRotationControl { get; set; }

        public int ClientTime { get; set; }

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(PlayerEntityId);
            writer.Push(position);
            writer.Push(velocity);
            writer.Push(angularVelocity);
            writer.Push(rotation);
            writer.Push(turretRotation);
            writer.Push(WeaponRotation);
            writer.Push(TankMoveControl);
            writer.Push(TankTurnControl);
            writer.Push(WeaponRotationControl);
            writer.Push(ClientTime);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            PlayerEntityId = reader.ReadInt64();
            position = reader.ReadVector3();
            velocity = reader.ReadVector3();
            angularVelocity = reader.ReadVector3();
            rotation = reader.ReadQuaternion3();
            turretRotation = reader.ReadQuaternion3();
            WeaponRotation = reader.ReadFloat();
            TankMoveControl = reader.ReadFloat();
            TankTurnControl = reader.ReadFloat();
            WeaponRotationControl = reader.ReadFloat();
            ClientTime = (int)reader.ReadInt64();
        }
    }
}
