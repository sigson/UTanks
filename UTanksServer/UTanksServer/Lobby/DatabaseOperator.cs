using System.Data.SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTanksServer.Database
{
    public static class DatabaseQuery
    {
        static public string CreateUsers = @"
CREATE TABLE IF NOT EXISTS  ""Users"" (
	""id""	INTEGER NOT NULL,
	""Username""	VARCHAR(20) NOT NULL COLLATE NOCASE,
	""Password""	VARCHAR(44) NOT NULL,
	""Email""	TEXT,
    ""EmailVerified""	TINYINT(1) NOT NULL DEFAULT 0,
	""RegistrationDate""	TEXT NOT NULL,
	""UserGroup""	TEXT NOT NULL DEFAULT 'admin',
	""Clan""	INTEGER,
	""Rank""	INTEGER NOT NULL DEFAULT 0,
	""GlobalScore""	INTEGER NOT NULL DEFAULT 0,
	""RankScore""	INTEGER NOT NULL DEFAULT 0,
	""Crystalls""	INTEGER NOT NULL DEFAULT 100,
	""LastDatetimeGetDailyBonus""	TEXT,
	""GarageJSONData""	TEXT NOT NULL,
	""LastIp""	TEXT NOT NULL DEFAULT '127.0.0.1',
	""TermlessBan""	INTEGER NOT NULL DEFAULT 0,
	""ActiveChatBanEndTime""	TEXT,
	""UserLocation""	TEXT NOT NULL DEFAULT 'ru',
	""Karma""	INTEGER NOT NULL DEFAULT 0,
	""HardwareId""	INTEGER,
	""HardwareToken""	INTEGER,
    FOREIGN KEY(""Clan"") REFERENCES ""Clans""(""id""),
	PRIMARY KEY(""id"")
);
";

        static public string CreateClans = @"
CREATE TABLE IF NOT EXISTS  ""Clans"" (
	""id""	INTEGER NOT NULL,
	""ClanName""	INTEGER,
	""ClanIcon""	TEXT,
	""ClanColormapTexture""	TEXT,
	""ClanPlayer""	INTEGER,
	FOREIGN KEY(""ClanPlayer"") REFERENCES ""Users""(""id""),
	PRIMARY KEY(""id"")
);
";

        static public string CreateKarma = @"
CREATE TABLE IF NOT EXISTS ""Karma"" (
	""id""	INTEGER NOT NULL,
	""UserId""	INTEGER NOT NULL,
	""ModerBannedChat""	INTEGER,
	""ModerBannedGame""	INTEGER,
	""ReasonChatBan""	TEXT,
	""ReasonGameBan""	TEXT,
	""EndChatBannedTime""	TEXT,
	""EndGameBannedTime""	TEXT,
	FOREIGN KEY(""ModerBannedChat"") REFERENCES ""Users""(""id""),
	FOREIGN KEY(""ModerBannedGame"") REFERENCES ""Users""(""id""),
	FOREIGN KEY(""UserId"") REFERENCES ""Users""(""id""),
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateNews = @"
CREATE TABLE IF NOT EXISTS ""News"" (
	""id""	INTEGER NOT NULL,
	""Date""	TEXT NOT NULL,
	""Header""	TEXT NOT NULL,
	""Text""	TEXT NOT NULL,
	""Icon""	TEXT NOT NULL DEFAULT 'https://myserver.com/img.jpg',
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateMapList = @"
CREATE TABLE IF NOT EXISTS ""MapList"" (
	""id""	INTEGER NOT NULL,
	""Name""	INTEGER,
	""MinAccessRank""	INTEGER,
	""MaxAccessRank""	INTEGER,
	""MaxPlayers""	INTEGER,
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateBattleModes = @"
CREATE TABLE IF NOT EXISTS ""BattleModes"" (
	""id""	INTEGER NOT NULL,
	""Name""	TEXT,
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateMapVariants = @"
CREATE TABLE IF NOT EXISTS ""MapVariants"" (
	""id""	INTEGER NOT NULL,
	""MapId""	INTEGER NOT NULL,
	""BattleMode""	INTEGER NOT NULL,
	""HiddenMap""	INTEGER NOT NULL DEFAULT 0,
	""MapPath""	TEXT NOT NULL,
	""SkyboxPath""	TEXT NOT NULL,
	""MusicThemePath""	TEXT NOT NULL,
	""SpawnZoneJson""	TEXT,
	""BonusZoneJson""	TEXT,
	""FlagsFlagpolePositionJson""	TEXT,
	FOREIGN KEY(""BattleMode"") REFERENCES ""BattleModes""(""id""),
	FOREIGN KEY(""MapId"") REFERENCES ""MapList""(""id""),
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateLogs = @"
CREATE TABLE IF NOT EXISTS ""Logs"" (
	""id""	INTEGER NOT NULL,
	""Date""	TEXT NOT NULL,
	""Type""	TEXT NOT NULL,
	""Message""	TEXT NOT NULL,
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateInvites = @"
CREATE TABLE IF NOT EXISTS ""Invites"" (
	""id""	INTEGER NOT NULL,
	""UserId""	INTEGER NOT NULL,
	""Code""	TEXT NOT NULL,
	FOREIGN KEY(""UserId"") REFERENCES ""Users""(""id""),
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string CreateFriends = @"
CREATE TABLE IF NOT EXISTS ""Friends"" (
	""id""	INTEGER NOT NULL,
	""UserId""	INTEGER NOT NULL,
	""FriendId""	INTEGER NOT NULL,
	FOREIGN KEY(""UserId"") REFERENCES ""Users""(""id""),
	FOREIGN KEY(""FriendId"") REFERENCES ""Users""(""id""),
	PRIMARY KEY(""id"" AUTOINCREMENT)
);
";

        static public string NewbieAccountGarage = "";//loading from newbie.json
    }

    public static class DatabaseOperator
    {
        //public string pathToDB;

        //public DatabaseOperator(string PathToDB)
        //{
        //    pathToDB = PathToDB;
        //}

        //async public static void CheckDatabase(SQLiteConnection connection)
        //{
        //    connection.Open();
        //    SQLiteCommand command = new SQLiteCommand();
        //    command.Connection = connection;
        //    command.CommandText = "SELECT * FROM sqlite_master WHERE name ='Users' and type='table';";
        //    using (SQLiteDataReader reader = command.ExecuteReaderAsync())
        //    {
        //        if (!reader.HasRows)
        //        {
        //            CreateDatabase(connection);
        //        }
        //    }
        //}

        async public static void CreateDatabase(SQLiteConnection connection)
        {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connection;
            command.CommandText = DatabaseQuery.CreateUsers;
            command.CommandText += DatabaseQuery.CreateKarma;
            command.CommandText += DatabaseQuery.CreateNews;
            command.CommandText += DatabaseQuery.CreateMapList;
            command.CommandText += DatabaseQuery.CreateBattleModes;
            command.CommandText += DatabaseQuery.CreateMapVariants;
            command.CommandText += DatabaseQuery.CreateLogs;
            command.CommandText += DatabaseQuery.CreateInvites;
            command.CommandText += DatabaseQuery.CreateFriends;
            command.ExecuteNonQuery();
        }

        public static List<List<dynamic>> ReadData(SQLiteConnection connection, string query)
        {
            List<List<dynamic>> result = new List<List<dynamic>>();
            connection.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connection;
            command.CommandText = query;
            using (SQLiteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        List<dynamic> row = new List<dynamic>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader.GetValue(i));
                        }
                        result.Add(row);
                    }
                }
            }
            return result;
        }

        public static void WriteData(SQLiteConnection connection, string query)
        {
            connection.Open();
            SQLiteCommand command = new SQLiteCommand();
            command.Connection = connection;
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
    }
}
