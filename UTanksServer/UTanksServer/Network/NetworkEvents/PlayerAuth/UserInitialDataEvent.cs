using UTanksServer.Network.Simple.Net;

namespace UTanksServer.Network.NetworkEvents.PlayerAuth
{
    public struct UserInitialDataEvent : INetSerializable
    {
        public int packetId;
        public long uid;
        public string Username;
        public string Password;
        public string Email;
        public bool EmailVerified;
        public long RegistrationDate;
        public string UserGroup;
        public int Clan;
        public int Rank;
        public int GlobalScore;
        public int RankScore;
        public int Crystalls;
        public long LastDatetimeGetDailyBonus;
        public string GarageJSONData;
        public string LastIp;
        public bool TermlessBan;
        public long ActiveChatBanEndTime;
        public string UserLocation;
        public int Karma;
        public string HardwareId;
        public string HardwareToken;

        public void Serialize(NetWriter writer)
        {
            writer.Push(packetId);
            writer.Push(uid);
            if (uid == -1) return;
            writer.Push(Username);
            writer.Push(Email);
            writer.Push(EmailVerified);
            writer.Push(RegistrationDate);
            writer.Push(UserGroup);
            writer.Push(Clan);
            writer.Push(Rank);
            writer.Push(GlobalScore);
            writer.Push(RankScore);
            writer.Push(Crystalls);
            writer.Push(LastDatetimeGetDailyBonus);
            writer.Push(GarageJSONData);
            writer.Push(TermlessBan);
            writer.Push(ActiveChatBanEndTime);
            writer.Push(UserLocation);
            writer.Push(Karma);
            writer.Push(HardwareId);
            writer.Push(HardwareToken);
        }

        public void Deserialize(NetReader reader)
        {
            packetId = (int)reader.ReadInt64();
            uid = reader.ReadInt64();
            if (uid == -1) return;
            Username = reader.ReadString();
            Email = reader.ReadString();
            EmailVerified = reader.ReadBool();
            UserGroup = reader.ReadString();
            Clan = (int)reader.ReadInt64();
            Rank = (int)reader.ReadInt64();
            GlobalScore = (int)reader.ReadInt64();
            RankScore = (int)reader.ReadInt64();
            Crystalls = (int)reader.ReadInt64();
            GarageJSONData = reader.ReadString();
            TermlessBan = reader.ReadBool();
            ActiveChatBanEndTime = reader.ReadInt64();
            UserLocation = reader.ReadString();
            Karma = (int)reader.ReadInt64();
            HardwareId = reader.ReadString();
            HardwareToken = reader.ReadString();
        }
    }
}
