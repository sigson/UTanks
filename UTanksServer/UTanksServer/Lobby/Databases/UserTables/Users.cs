using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using Core;

namespace UTanksServer.Database.Databases.UserTables {
    public class Users {
        //public string Escape(string value) => Db
        public async Task<Users> Init() {
            using (SQLiteCommand request = new SQLiteCommand(
                DatabaseQuery.CreateUsers + DatabaseQuery.CreateKarma + DatabaseQuery.CreateNews + DatabaseQuery.CreateMapList + DatabaseQuery.CreateBattleModes + DatabaseQuery.CreateMapVariants + DatabaseQuery.CreateLogs + DatabaseQuery.CreateInvites + DatabaseQuery.CreateFriends, UserDatabase.Connection))
                await request.ExecuteNonQueryAsync();
            Logger.Log("Table 'UserDatabase.users` initilized", "UserDB");
            return this;
        }

        public async Task<bool> UsernameAvailable(string username) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT id FROM Users WHERE Username = '{username}' COLLATE NOCASE;",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                bool result = !response.HasRows;
                await response.CloseAsync();
                return result;
            }
        }

        public async Task<bool> LoginCheck(string username, string hashedPassword)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT id FROM Users WHERE Username = '{username}' AND Password = '{hashedPassword}' COLLATE NOCASE;",
                UserDatabase.Connection
            ))
            {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                bool result = response.HasRows;
                await response.CloseAsync();
                return result;
            }
        }

        public async Task<bool> EmailAvailable(string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT id FROM Users WHERE Email = '{email}';",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                bool result = !response.HasRows;
                await response.CloseAsync();
                return result;
            }
        }

        public async Task<UserRow> Create(string username, string hashedPassword, string email = null, string hardwareId = null, string token = null) {
            if (email == null) email = string.Empty;
            if (hardwareId == null) hardwareId = string.Empty;
            if (token == null) token = string.Empty;
            else if (!await EmailAvailable(email)) throw new ArgumentException("Email Taken!");
            using (SQLiteCommand request = new SQLiteCommand(
                $"INSERT INTO Users(Username, Password, Email, HardwareId, HardwareToken, GarageJSONData, RegistrationDate) VALUES('{username}', '{hashedPassword}', '{email}', '{hardwareId}', '{token}', '{DatabaseQuery.NewbieAccountGarage}', '{DateTime.Now.Ticks}');",
                UserDatabase.Connection
            )) {
                await request.ExecuteNonQueryAsync();

                return await GetUserViaCallsign(username);
            }
        }

        public async Task<UserRow> GetUserViaCallsign(string username) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT * FROM Users WHERE Username = '{username}'",
                UserDatabase.Connection
            )) {
                using (DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow)) {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    if (!response.HasRows) {
                        result.Add("error", "User not found");
                        return UserRow.Empty;
                    }
                    
                    await response.ReadAsync();

                    return new UserRow() {
                        id = response.GetInt32("id"),
                        Username = response.GetString("Username"),
                        Password = response.GetString("Password"),
                        Email = response.GetString("Email"),
                        EmailVerified = (response.GetString("EmailVerified") == ""? false : true),
                        Clan = response.GetInt32("Clan"),
                        Crystalls = response.GetInt32("Crystalls"),
                        Rank = response.GetInt32("Rank"),
                        GlobalScore = response.GetInt32("GlobalScore"),
                        RankScore = response.GetInt32("RankScore"),
                        Karma = response.GetInt32("Karma"),
                        TermlessBan = (response.GetInt32("TermlessBan") == 0 ? false : true),
                        HardwareId = response.GetString("HardwareId"),
                        HardwareToken = response.GetString("HardwareToken"),
                        ActiveChatBanEndTime = long.Parse((response.GetString("ActiveChatBanEndTime") == ""? "-1": response.GetString("ActiveChatBanEndTime"))),
                        GarageJSONData = response.GetString("GarageJSONData"),
                        LastDatetimeGetDailyBonus = long.Parse((response.GetString("LastDatetimeGetDailyBonus") == "" ? "-1" : response.GetString("LastDatetimeGetDailyBonus"))),
                        LastIp = response.GetString("LastIp"),
                        RegistrationDate = long.Parse(response.GetString("RegistrationDate")),
                        UserGroup = response.GetString("UserGroup"),
                        UserLocation = response.GetString("UserLocation")
                    };
                }
            }
        }

        public async Task<Dictionary<string, object>> GetUserViaEmail(string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT * FROM Users WHERE Email = '{email}'",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                Dictionary<string, object> result = new Dictionary<string, object>();
                if (response.HasRows) {
                    await response.ReadAsync();

                    result.Add("id", (long)response.GetInt32("id"));
                    result.Add("Username", response.GetString("Username"));
                    result.Add("Password", response.GetString("Password"));
                    result.Add("Email", response.GetString("Email"));
                    result.Add("EmailVerified", response.GetBoolean("EmailVerified"));
                    result.Add("Clan", response.GetInt32("Clan"));
                    result.Add("Crystalls", response.GetInt32("Crystalls"));
                    result.Add("Rank", response.GetInt32("Rank"));
                    result.Add("GlobalScore", response.GetInt32("GlobalScore"));
                    result.Add("RankScore", response.GetInt32("RankScore"));
                    result.Add("Karma", response.GetInt32("Karma"));
                    result.Add("TermlessBan", (response.GetInt32("TermlessBan") == 0 ? false : true));
                    result.Add("HardwareId", response.GetString("HardwareId"));
                    result.Add("HardwareToken", response.GetString("HardwareToken"));
                    result.Add("ActiveChatBanEndTime", response.GetString("ActiveChatBanEndTime"));
                    result.Add("GarageJSONData", response.GetString("GarageJSONData"));
                    result.Add("LastDatetimeGetDailyBonus", response.GetString("LastDatetimeGetDailyBonus"));
                    result.Add("LastIp", response.GetString("LastIp"));
                    result.Add("RegistrationDate", response.GetString("RegistrationDate"));
                    result.Add("UserGroup", response.GetString("UserGroup"));
                    result.Add("UserLocation", response.GetString("UserLocation"));
                }
                else result.Add("error", "User not found");
                await response.CloseAsync();

                return result;
            }
        }

        public async Task<Queue<string>> GetEmailList() {
            using (SQLiteCommand request = new SQLiteCommand(
                "SELECT Users.Email AS Email FROM Users, `user-settings` WHERE users.uid = `user-settings`.uid AND email != '' AND `user-settings`.subscribed AND Users.EmailVerified;",
                UserDatabase.Connection
            )) {
                Queue<string> result = new Queue<string>();
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.Default);

                while (response.HasRows) {
                    await response.ReadAsync();
                    result.Enqueue(response.GetString("Email"));
                    await response.NextResultAsync();
                }

                return result;
            }
        }

        /// <summary>
        /// Will change the username of a user, returns true if the operation was successful
        /// </summary>
        public async Task<bool> SetUsername(long uid, string newUsername) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET Username = '{newUsername}' WHERE id = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetHashedPassword(long uid, string hashedPassword) {
            if (hashedPassword.Length > 44) throw new ArgumentException("Parameter 'hashedPassword' is too long!");

            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET Password = '{hashedPassword}' WHERE id = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        // Will set the emailVerified = false
        public async Task<bool> SetEmail(long uid, string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET Email = '{email}', EmailVerified = 0 WHERE id = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetEmailVerified(long uid, bool value) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET EmailVerified = {(value ? 1 : 0)} WHERE id = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetHardwareId(long uid, string hardwareId) {
            if (hardwareId.Length > 100) throw new ArgumentException("Parameter hardwareId cannot not be over 100 characters");
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET HardwareId = '{hardwareId}' WHERE id = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetRememberMe(long uid, string hardwareId, string token) {
            if (hardwareId.Length > 100) throw new ArgumentException("Parameter hardwareId cannot not be over 100 characters");
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE Users SET HardwareId = '{hardwareId}', HardwareToken = '{token}' WHERE id = {uid};",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }
    }
}