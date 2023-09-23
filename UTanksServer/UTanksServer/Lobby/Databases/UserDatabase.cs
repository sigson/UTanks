using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using UTanksServer.Database.Databases.UserTables;
using Core;

namespace UTanksServer.Database.Databases {
    public static class UserDatabase {
        public static SQLiteConnection Connection;
        public static Users Users;
        public static UserSettings UserSettings;
        //public static UserSettings UserSettings;

        public static async void Load() {
            string connectionString = "URI=file:" + Path.Join(GlobalProgramState.ConfigDir, "Users.db");
            Logger.LogDebug($"Using DB on path => '{connectionString}'", "UserDB");
            Connection = new SQLiteConnection(connectionString);
            await Connection.OpenAsync();
            Users = await new Users().Init();
            //UserSettings = await new UserSettings().Init();
            //UserSettings = await new UserSettings().Init();
        }

        public static async void Dispose() {
            Users = null;
            await Connection.CloseAsync();
        }
    }
    public struct UserRow {
        public static UserRow Empty = new UserRow() { id = -1 };
        public long id;
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


        public override bool Equals(object obj)
            => Equals((UserRow)obj);

        public bool Equals(UserRow other)
            => this == other;

        public static bool operator ==(UserRow lhs, UserRow rhs)
            => lhs.id == rhs.id;

        public static bool operator !=(UserRow lhs, UserRow rhs)
            => !(lhs == rhs);

        public override string ToString()
            => $"{{" +
               $"\n  uuid            => {id}" +
               $"\n  username        => {Username}" +
               $"\n  hashedPassword  => {Password}" +
               $"\n  email           => {Email}" +
               $"\n  emailverified   => {EmailVerified}" +
               $"\n  hardwareId      => {HardwareId}" +
               $"\n  hardwareToken   => {HardwareToken}" +
               $"\n}}";

        public override int GetHashCode()
        {
            int hashCode = 1975335690;
            hashCode *= -1521134295 + id.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Username);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Password);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(Email);
            hashCode *= -1521134295 + EqualityComparer<bool>.Default.GetHashCode(EmailVerified);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(HardwareId);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(HardwareToken);
            
            return hashCode;
        }
    }
}