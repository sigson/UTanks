using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;
using Core;

namespace UTanksServer.Database.Databases.ServerTables
{
    public class Servers
    {
        public async Task<Servers> Init()
        {
            using (SQLiteCommand request = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS `servers` (" +
                "  `uid` INTEGER PRIMARY KEY AUTOINCREMENT," +
                "  `name` TEXT NOT NULL UNIQUE COLLATE NOCASE," +
                "  `key` TEXT NOT NULL UNIQUE COLLATE NOCASE," +
                "  `token` TEXT NOT NULL COLLATE NOCASE," +
                "  `addresses` TEXT NOT NULL COLLATE NOCASE" +
                ");", ServerDatabase.Connection))
                await request.ExecuteNonQueryAsync();
            Logger.Log("Table 'ServerDatabase.servers' initilized", "init");
            return this;
        }

        public async Task<ServerRow> Get(string APIKey)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT * FROM servers where key = '{APIKey}';",
                ServerDatabase.Connection
            ))
            {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                if (!response.HasRows)
                    return ServerRow.Empty;

                await response.ReadAsync();
                List<string> addresses = new List<string>();
                addresses.AddRange(response.GetString("addresses").Split(' '));
                ServerRow result = new ServerRow()
                {
                    uid = response.GetInt32("uid"),
                    name = response.GetString("name"),
                    key = response.GetString("key"),
                    token = response.GetString("token"),
                    addresses = addresses
                };
                response.Close();
                return result;
            }
        }

        public async Task<bool> Save(ServerRow data)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET" +
                $" name = '{data.name}'," +
                $" key = {data.key}," +
                $" token = '{data.token}'," +
                $" addresses = '{data.addresses}'" +
                $" WHERE uid = {data.uid};",
                ServerDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> Create(ServerRow data)
        {
            using (SQLiteCommand request = new SQLiteCommand(
                $"INSERT INTO servers(name, key, token, addresses) VALUES(" +
                $" '{data.name}'," +
                $" '{data.key}'," +
                $" '{data.token}'," +
                $" '{data.addresses}'" +
                $");",
                ServerDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }
    }
}