using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksServer.Extensions;

//namespace UTanksServer.Extensions
//{
    public static class EnumerableExtension
    {
        public static void ForEach<TKey>(this IEnumerable<TKey> enumerable, Action<TKey> compute)
        {
            Collections.ForEach<TKey>(enumerable, compute);
        }

        public static void ForEach<TKey>(this IList<TKey> list, Action<TKey> compute)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                compute(list[i]);
            }
        }

        public static long GuidToLong(this Guid guid)
        {
            return DateTime.UtcNow.Ticks + BitConverter.ToInt64(guid.ToByteArray(), 8);
        }
    }
//}
