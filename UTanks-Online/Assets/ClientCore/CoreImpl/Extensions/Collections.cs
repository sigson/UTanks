using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UTanksClient.Extensions
{
    public class TupleList<T1, T2> : List<Tuple<T1, T2>> where T1 : IComparable
    {
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }

        public new void Sort()
        {
            Comparison<Tuple<T1, T2>> c = (a, b) => a.Item1.CompareTo(b.Item1);
            base.Sort(c);
        }
        public void ReverseSort()
        {
            Comparison<Tuple<T1, T2>> c = (a, b) => b.Item1.CompareTo(a.Item1);
            base.Sort(c);
        }

    }
    public class DescComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            if (x == null) return -1;
            if (y == null) return 1;
            return Comparer<T>.Default.Compare(y, x);
        }
    }
    public static class Collections
    {
        public static readonly object[] EmptyArray = new object[0];

        public static IEnumerable<T> IfThenElse<T>(
    this IEnumerable<T> elements,
    Func<bool> condition,
    Func<IEnumerable<T>, IEnumerable<T>> thenPath,
    Func<IEnumerable<T>, IEnumerable<T>> elsePath)
        {
            return condition()
                ? thenPath(elements)
                : elsePath(elements);
        }

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
            foreach (var row in dictionary)
            {
                this.TryAdd(row.Key, row.Value);
            }
        }
    }

    public class ConcurrentHashSet<T> : ICollection<T>, IEnumerable<T>, System.Collections.IEnumerable, IReadOnlyCollection<T>, ISet<T>, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable
    {
        private ConcurrentDictionary<T, int> storage = new ConcurrentDictionary<T, int>();


        public int Count => storage.Count;

        public bool IsReadOnly => storage.Keys.IsReadOnly;

        public void Add(T item)
        {
            storage[item] = 0;
        }

        public void Clear()
        {
            storage.Clear();
        }

        public bool Contains(T item)
        {
            return storage.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return storage.Keys.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void OnDeserialization(object sender)
        {
            throw new NotImplementedException();
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            return storage.TryRemove(item, out _);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        public void UnionWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        bool ISet<T>.Add(T item)
        {
            return storage.TryAdd(item, 0);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return storage.Keys.GetEnumerator();
        }
    }

    public class ConcurrentList<T> : IList<T> where T : class
    {
        private readonly ConcurrentDictionary<long, T> _store;

        public ConcurrentList(IEnumerable<T> items = null)
        {
            var prime = (items ?? Enumerable.Empty<T>()).Select(x => new KeyValuePair<long, T>(Guid.NewGuid().GuidToLong(), x));
            _store = new ConcurrentDictionary<long, T>(prime);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _store.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            if (_store.TryAdd(Guid.NewGuid().GuidToLong(), item) == false)
                throw new ApplicationException("Unable to concurrently add item to list");
        }

        public void Clear()
        {
            _store.Clear();
        }

        public bool Contains(T item)
        {
            return _store.Values.Where(x => item == x).Count() > 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _store.Values.CopyTo(array, arrayIndex);
        }

        public T[] ToArray()
        {
            return _store.Values.ToArray();
        }

        public bool Remove(T item)
        {
            foreach(var key in _store.Keys)
            {
                if (_store.TryGetValue(key, out var value) && value == item)
                    return _store.TryRemove(key, out _);
            }
            return false;
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _store.Count; }
        }

        public bool IsReadOnly
        {
            get { return _store.Keys.IsReadOnly; }
        }

        public T this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
