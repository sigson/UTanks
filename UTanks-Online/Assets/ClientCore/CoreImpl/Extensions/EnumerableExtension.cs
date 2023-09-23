using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTanksClient.Extensions;

namespace UTanksClient
{
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

        public static T Fill<T>(this T fillObject, System.Collections.IEnumerable fillInput, Action<T, object> fillAction) where T : System.Collections.IEnumerable
        {
            foreach (var fillObj in fillInput)
                fillAction(fillObject, fillObj);
            return fillObject;
        }

        public static long GuidToLong(this Guid guid)
        {
            return DateTime.UtcNow.Ticks + BitConverter.ToInt64(guid.ToByteArray(), 8);
        }
    }
}
