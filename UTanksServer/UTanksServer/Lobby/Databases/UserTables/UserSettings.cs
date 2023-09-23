using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using Core;
using UTanksServer.Network.NetworkEvents.PlayerSettings;

namespace UTanksServer.Database.Databases.UserTables
{
    public class UserSettings
    {
        public async Task<UserSettings> Init()
        {
            //using (SQLiteCommand request = new SQLiteCommand(
            //    "CREATE TABLE IF NOT EXISTS `user-settings`(" +
            //    " uid INTEGER NOT NULL," +
            //    " countryCode VARCHAR(2) NOT NULL DEFAULT 'EN', " +
            //    " avatar VARCHAR(36) NOT NULL DEFAULT '8b74e6a3-849d-4a8d-a20e-be3c142fd5e8', " +
            //    " premiumExpiration BIGINT NOT NULL DEFAULT 0, " +
            //    " subscribed TINYINT(1) NOT NULL DEFAULT 0" +
            //    ");",
            //    UserDatabase.Connection))
            //{
            //    await request.ExecuteNonQueryAsync();
            Logger.Log("Table 'UserDatabase.user-settings` initilized", "UserDB");
            return this;
            //}
        }

        public async Task<UserSettingsData> Get(long uid)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT countryCode, avatar, premiumExpiration, subscribed FROM `user-settings` WHERE uid = {uid}",
                UserDatabase.Connection
            ))
            {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);

                if (response.HasRows)
                {
                    await response.ReadAsync();

                    UserSettingsData result = new UserSettingsData()
                    {
                        countryCode = response.GetString("countryCode"),
                        avatar = response.GetString("avatar"),
                        premiumExpiration = response.GetInt64("premiumExpiration"),
                        subscribed = response.GetBoolean("subscribed")
                    };
                    await response.CloseAsync();
                    return result;
                }
                await response.CloseAsync();

                // Set
                using (SQLiteCommand setRequest = new SQLiteCommand(
                    $"INSERT INTO `user-settings`(uid, premiumExpiration) VALUES({uid}, '{DateTime.MinValue.Ticks}')",
                    UserDatabase.Connection
                )) await setRequest.ExecuteNonQueryAsync();

                // Then get again
                response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (response.HasRows)
                {
                    await response.ReadAsync();
                    UserSettingsData result = new UserSettingsData()
                    {
                        countryCode = response.GetString("countryCode"),
                        avatar = response.GetString("avatar"),
                        premiumExpiration = response.GetInt64("premiumExpiration"),
                        subscribed = response.GetBoolean("subscribed")
                    };
                    await response.CloseAsync();

                    return result;
                }

                throw new Exception("Kaveman, fix your user settings getter function :[");
            }
        }

        public async Task<bool> SetCountryCode(long uid, string countryCode)
        {
            if (countryCode.Length > 2) throw new ArgumentException("Paramter `countryCode` is too long!");

            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE `user-settings` SET countryCode = '{countryCode}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetAvatar(long uid, string avatar)
        {
            if (avatar.Length > 36) throw new ArgumentException("Parameter 'avatar' is too long!");

            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE `user-settings` SET avatar = '{avatar}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetPremiumExpiration(long uid, long utcTicks)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE `user-settings` SET premiumExpiration = '{utcTicks}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetSubscribedState(long uid, bool value)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE `user-settings` SET subscribed = {(value ? 1 : 0)} WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }
    }
}
