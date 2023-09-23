using System;
using System.Linq;
using System.Collections.Generic;

namespace UTanksClient.Network.Simple.Net {
    public static class HashCache {
        static Dictionary<Type, long> Cache = new Dictionary<Type, long>();
        /*private static class HashCache<T> {
            public static bool Initialized;
            public static long Id;
            public static Type type;
        }*/

        public static long Get<T>() where T : struct, INetSerializable {
            Type type = typeof(T);
            if (Cache.ContainsKey(type))
                return Cache[type];

            ulong hash = 14695981039346656037UL; //offset
            string typeName = typeof(T).Name;//.FullName;
            for (var i = 0; i < typeName.Length; i++) {
                hash = hash ^ typeName[i];
                hash *= 1099511628211UL; //prime
            }
            long signedHash = unchecked((long)hash + long.MinValue);;
            Cache.Add(type, signedHash);
            return signedHash;
        }

        public static Type GetType(long hashCode) {
            if (!Cache.ContainsValue(hashCode))
                return null;
            return Cache.FirstOrDefault(x => x.Value == hashCode).Key;
        }
    }
}