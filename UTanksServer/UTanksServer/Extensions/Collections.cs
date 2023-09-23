using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UTanksServer.Extensions
{
    public class Collections
    {
        public static readonly object[] EmptyArray = new object[0];

        public static List<T> AsList<T>(params T[] values) =>
            new List<T>(values);

        //public static IList<T> EmptyList<T>() =>
        //    EmptyList<T>.Instance;

        public static void ForEach<T>(IEnumerable<T> coll, Action<T> action)
        {
            Enumerator<T> enumerator = GetEnumerator<T>(coll);
            while (enumerator.MoveNext())
            {
                action(enumerator.Current);
            }
        }

        public static IEnumerable<TSource> IntersectEnum<TSource>(HashSet<TSource> first, IEnumerable<TSource> second)
        {
            foreach (TSource element in first)
            {
                if (second.Contains(element)) yield return element;
            }
        }

        public static bool FirstIntersect<TSource, TNull>(ConcurrentDictionaryEx<TSource, TNull> first, IEnumerable<TSource> second)
        {
            foreach (KeyValuePair<TSource, TNull> element in first)
            {
                if (second.Contains(element.Key)) return true;
            }
            return false;
        }

        public static IEnumerable<TSource> IntersectEnum<TSource, TNull>(ConcurrentDictionaryEx<TSource, TNull> first, IEnumerable<TSource> second)
        {
            foreach (KeyValuePair<TSource, TNull> element in first)
            {
                if (second.Contains(element.Key)) yield return element.Key;
            }
        }

        public static bool FirstIntersect<TSource>(HashSet<TSource> first, IEnumerable<TSource> second)
        {
            foreach (TSource element in first)
            {
                if (second.Contains(element)) return true;
            }
            return false;
        }

        public static Enumerator<T> GetEnumerator<T>(IEnumerable<T> collection) =>
            new Enumerator<T>(collection);

        public static T GetOnlyElement<T>(ICollection<T> coll)
        {
            if (coll.Count != 1)
            {
                throw new InvalidOperationException("Count: " + coll.Count);
            }
            List<T> list = coll as List<T>;
            if (list != null)
            {
                return list[0];
            }
            HashSet<T> set = coll as HashSet<T>;
            if (set != null)
            {
                HashSet<T>.Enumerator enumerator = set.GetEnumerator();
                enumerator.MoveNext();
                return enumerator.Current;
            }
            IEnumerator<T> enumerator2 = coll.GetEnumerator();
            enumerator2.MoveNext();
            return enumerator2.Current;
        }

        //public static IList<T> SingletonList<T>(T value) =>
        //    new SingletonList<T>(value);


        public struct Enumerator<T>
        {
            private IEnumerable<T> collection;
            private HashSet<T>.Enumerator hashSetEnumerator;
            private List<T>.Enumerator ListEnumerator;
            private IEnumerator<T> enumerator;
            public Enumerator(IEnumerable<T> collection)
            {
                this.collection = collection;
                this.enumerator = null;
                List<T> list = collection as List<T>;
                if (list != null)
                {
                    this.ListEnumerator = list.GetEnumerator();
                    HashSet<T>.Enumerator enumerator = new HashSet<T>.Enumerator();
                    this.hashSetEnumerator = enumerator;
                }
                else
                {
                    HashSet<T> set = collection as HashSet<T>;
                    if (set != null)
                    {
                        this.hashSetEnumerator = set.GetEnumerator();
                        List<T>.Enumerator enumerator2 = new List<T>.Enumerator();
                        this.ListEnumerator = enumerator2;
                    }
                    else
                    {
                        HashSet<T>.Enumerator enumerator3 = new HashSet<T>.Enumerator();
                        this.hashSetEnumerator = enumerator3;
                        List<T>.Enumerator enumerator4 = new List<T>.Enumerator();
                        this.ListEnumerator = enumerator4;
                        this.enumerator = collection.GetEnumerator();
                    }
                }
            }

            public bool MoveNext() =>
                !(this.collection is List<T>) ? (!(this.collection is HashSet<T>) ? this.enumerator.MoveNext() : this.hashSetEnumerator.MoveNext()) : this.ListEnumerator.MoveNext();

            public T Current =>
                !(this.collection is List<T>) ? (!(this.collection is HashSet<T>) ? this.enumerator.Current : this.hashSetEnumerator.Current) : this.ListEnumerator.Current;
        }
    }

    public class ConcurrentDictionaryEx<TKey, TValue> : ConcurrentDictionary<TKey, TValue>
    {
        public int FastCount;

        public IList<TKey> IKeys = new List<TKey>();
        public IList<TValue> IValues = new List<TValue>();


        public ConcurrentDictionaryEx() : base()
        {

        }

        public ConcurrentDictionaryEx<TKey, TValue> Upd()
        {
            for (int i = 0; i < IKeys.Count; i++)
            {
                this.TryAdd(IKeys[i], IValues[i]);
            }
            return this;
        }

        public ConcurrentDictionaryEx(IDictionary<TKey, TValue> dictionary)
        {
            foreach(var row in dictionary)
            {
                this.TryAdd(row.Key, row.Value);
            }
        }

    }

    public class ConcurrentHashSet<T> : HashSet<T>
    {
        public int FastCount;
        ConcurrentDictionary<T, int> dictionary;

        public new bool Add(T item)
        {
            dictionary.TryAdd(item, 0);
            Interlocked.Increment(ref FastCount);
            return true;
        }

        public new bool Remove(T item)
        {
            dictionary.TryRemove(new KeyValuePair<T, int>(item, 0));
            Interlocked.Decrement(ref FastCount);
            return true;
        }

        
        //public 
    }
}
