using System;
using System.IO;
using System.Data.SQLite;
using Core;
using UTanksServer.Database.Databases.ServerTables;
using System.Collections.Generic;

namespace UTanksServer.Database.Databases {
    public static class ServerDatabase {
        public static SQLiteConnection Connection;
        public static Servers Servers;

        public static async void Load() {
            string connectionString = "URI=file:" + Path.Join(GlobalProgramState.ConfigDir, "Servers.db");
            Logger.LogDebug($"Using DB on path => '{connectionString}'", "ServerDB");
            Connection = new SQLiteConnection(connectionString);
            await Connection.OpenAsync();
            Servers = await new Servers().Init();
        }

        public static async void Dispose() {
            Servers = null;
            await Connection.CloseAsync();
        }
    }
    public struct ServerRow {
        public static ServerRow Empty = new ServerRow() { uid = -1 };
        public long uid;
        public string name;
        public string key;
        public string token;
        public List<string> addresses;
        
        public override bool Equals(object obj)
            => Equals((ServerRow)obj);

        public bool Equals(ServerRow other)
            => this == other;

        public static bool operator ==(ServerRow lhs, ServerRow rhs)
            => lhs.uid == rhs.uid;

        public static bool operator !=(ServerRow lhs, ServerRow rhs)
            => !(lhs == rhs);

        public override string ToString() {
            try {
                return 
                $"{{" +
                  $"\n  uid             => {uid}" +
                  $"\n  name            => {name}" +
                  $"\n  key             => {key}" +
                  $"\n  token           => {token}" +
                  $"\n  addresses       => [{string.Join(", ", addresses)}]" +
                $"\n}}";
            } catch {}
            return "{ uid => -1 }";
        }

        public override int GetHashCode()
        {
            int hashCode = 1975335690;
            hashCode = hashCode * -1521134295 + uid.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(key);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(token);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<string>>.Default.GetHashCode(addresses);
            return hashCode;
        }
    }
}