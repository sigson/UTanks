using SecuredSpace.ClientControl.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UTanksClient.ClassExtensions;

public static class UnityObjectExtend
{
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        return (T)gameObject.GetOrAddComponent(typeof(T));
    }
    public static Component GetOrAddComponent(this GameObject gameObject, Type typeComponent)
    {
        var getComp = gameObject.GetComponent(typeComponent);
        if (getComp == null)
        {
            getComp = gameObject.AddComponent(typeComponent);
        }
        return getComp;
    }

    public static T GetOrAddComponent<T>(this Component gameObject) where T : Component
    {
        return (T)gameObject.GetOrAddComponent(typeof(T));
    }
    public static Component GetOrAddComponent(this Component gameObject, Type typeComponent)
    {
        var getComp = gameObject.GetComponent(typeComponent);
        if (getComp == null)
        {
            getComp = gameObject.gameObject.AddComponent(typeComponent);
        }
        return getComp;
    }

    public static T GetOrAddComponent<T>(this MonoBehaviour gameObject) where T : Component
    {
        return (T)gameObject.GetOrAddComponent(typeof(T));
    }
    public static Component GetOrAddComponent(this MonoBehaviour gameObject, Type typeComponent)
    {
        var getComp = gameObject.GetComponent(typeComponent);
        if (getComp == null)
        {
            getComp = gameObject.gameObject.AddComponent(typeComponent);
        }
        return getComp;
    }
}


namespace UTanksClient.ClassExtensions
{
    public static class MathEx
    {
        public static float Rad2Deg => 360f / ((float)Math.PI * 2);
        public static float RadToDeg(float rad)
        {
            return ((rad * MathEx.Rad2Deg) > 360 ? 360 - ((rad * MathEx.Rad2Deg) - 360) : 360 - (rad * MathEx.Rad2Deg)) - 180f;
        }

        public static void ResetForces(this Rigidbody rigidbody)
        {
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.velocity = Vector3.zero;
            rigidbody.ResetInertiaTensor();
        }

        public static Vector3 SetEx(this Vector3 vector, float? newX = null, float? newY = null, float? newZ = null)
        {
            vector.Set(newX != null ? (float)newX : vector.x, newY != null ? (float)newY : vector.y, newZ != null ? (float)newZ : vector.z);
            return vector;
        }
    }

    public class IText
    {
        private object OriginalText;
        private Type OriginalType;
        public IText(object TextObject)
        {
            this.TextObject = TextObject;
        }

        private static Dictionary<string, PropertyInfo> CachePropertyInfo = new Dictionary<string, PropertyInfo>();

        public object TextObject
        {
            get
            {
                return OriginalText;
            }
            set
            {
                OriginalText = value;
                OriginalType = OriginalText.GetType();
            }
        }

        public string text
        {
            get
            {
                return (string)CachedGetProperty("text").GetValue(this.TextObject);
            }
            set
            {
                CachedGetProperty("text").SetValue(this.TextObject, value);
            }
        }

        private PropertyInfo CachedGetProperty(string name)
        {
            PropertyInfo textProperty = null;
            if (!IText.CachePropertyInfo.TryGetValue(name + OriginalType.Name, out textProperty))
            {
                textProperty = OriginalType.GetProperty(name);
                IText.CachePropertyInfo[name + OriginalType.Name] = textProperty;
            }
            return textProperty;
        }
    }

    public class TimerEx : System.Timers.Timer
    {
        private long TimerStart = 0;
        private long TimerPaused = 0;
        private long TimerStopped = 0;
        private double baseInterval = 0f;
        private double interval = 0f;
        public bool inited = false;
        public bool Dead = false;
        public new double Interval
        {
            get
            {
                return interval;
            }
            set
            {
                if (value > 0)
                {
                    baseInterval = value;
                    interval = value;
                    base.Interval = value;
                }
            }
        }

        public TimerEx() : base()
        {
            this.Elapsed += (async (sender, e) => {
                if (base.AutoReset)
                    this.Interval = this.baseInterval;
                TimerStart = DateTime.Now.Ticks; TimerPaused = 0;
            });
            //base.AutoReset = true;
            base.Disposed += new EventHandler(this.OnDisposeTimer);
            this.Disposed += new EventHandler(this.OnDisposeTimer);
        }

        public TimerEx(TimerEx oldTimer) : base()
        {
            this.Elapsed += (async (sender, e) => {
                if (!base.AutoReset)
                    this.Interval = this.baseInterval;
                TimerStart = DateTime.Now.Ticks; TimerPaused = 0;
            });
            Interval = oldTimer.Interval;
            inited = true;
            //base.AutoReset = true;
            base.Disposed += new EventHandler(this.OnDisposeTimer);
            this.Disposed += new EventHandler(this.OnDisposeTimer);
        }

        public void OnDisposeTimer(object sender, EventArgs args)
        {
            Dead = true;
        }

        public new void Start()
        {
            TimerStart = DateTime.Now.Ticks;
            inited = true;
            base.Start();
        }

        public new void Stop()
        {
            TimerStopped = DateTime.Now.Ticks;
            base.Stop();
        }

        public void Pause()
        {
            if (this.Enabled)
            {
                this.Stop();
                TimerPaused = DateTime.Now.Ticks;
            }
        }

        public void Resume()
        {
            if (TimerPaused != 0 && !this.Enabled)
            {
                this.interval = TimerPaused - TimerStart;
                base.Interval = TimerPaused - TimerStart;
                TimerPaused = 0;
                TimerStart = DateTime.Now.Ticks;
                this.Start();
            }
        }

        public void Reset()
        {
            this.Interval = baseInterval;
            this.Stop();
            TimerStart = 0;
            TimerPaused = 0;
            TimerStopped = 0;
            this.Start();
        }

        public double RemainingToElapsedTime()
        {
            return baseInterval - TimeSpan.FromTicks((TimerPaused == 0 ? DateTime.Now.Ticks - TimerStart : TimerPaused - TimerStart)).TotalMilliseconds;
        }
    }
}

namespace UTanksClient.Extensions
{
    public static class AudioControl
    {
        public static AudioClip TrimAudioClip(AudioClip originalClip, float startPosSec, float lengthSec)
        {
            var originalClipSamples = new float[originalClip.samples];
            originalClip.GetData(originalClipSamples, 0);

            //converts startPosSec & takeAmountSec from seconds to sample amount
            int newStartPosSample = (int)(startPosSec * originalClip.frequency);
            int newLengthSecSample = (int)(lengthSec * originalClip.frequency);

            //gets the trimmed version of the orignalClipSamples
            var newClipSamples = originalClipSamples.Skip(newStartPosSample).Take(newLengthSecSample).ToArray();

            //generates a new empty clip and sets its data according to the newClipSamples
            AudioClip resClip = AudioClip.Create(originalClip.name, newClipSamples.Length, originalClip.channels, originalClip.frequency, false);
            resClip.SetData(newClipSamples, 0);

            return resClip;
        }

        public static AudioClip SnipAudioClip(AudioClip clipIn, int startSamples, int endSamples, bool loop)
        {
            int clipOutSamples;
            if (loop) { clipOutSamples = endSamples - startSamples; }
            else { clipOutSamples = startSamples; }


            AudioClip clipOut = AudioClip.Create("Trimmed " + clipIn.name, clipOutSamples, clipIn.channels, clipIn.frequency, false);
            float[] samplesToCopy = new float[clipOut.samples * clipOut.channels];

            int pointToGetData;

            if (loop) { pointToGetData = startSamples; }
            else { pointToGetData = 0; }
            clipIn.GetData(samplesToCopy, pointToGetData);
            clipOut.SetData(samplesToCopy, 0);

            return clipOut;
        }
    }

    public static class Lambda
    {
        public static UnityEvent<T> AddListener<T>(this UnityEvent<T> unityEvent, System.Action<T> action)
        {
            UnityAction<T> uaction = (T arg) => action(arg);
            unityEvent.AddListener(uaction);
            return unityEvent;
        }

        public static UnityEvent AddListener(this UnityEvent unityEvent, System.Action action)
        {
            UnityAction uaction = () => action();
            unityEvent.AddListener(uaction);
            return unityEvent;
        }

        public static bool TryExecute(Action act)
        {
            try
            {
                act();
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public static T LineFunction<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }
    }
    public static class stringEx
    {
        public static float FastFloat(this string str)
        {
            return float.Parse(str, System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public static class TextControl
    {
        public static string GetText(this TMP_Text text)
        {
            return text.text;
        }

        public static string GetText(this Text text)
        {
            return text.text;
        }

        public static void SetText(this TMP_Text text, string newText)
        {
            text.text = newText;
        }

        public static void SetText(this Text text, string newText)
        {
            text.text = newText;
        }


        public static IText GetAdapter(this TMP_Text text)
        {
            return new IText(text);
        }

        public static IText GetAdapter(this Text text)
        {
            return new IText(text);
        }
    }

    public abstract class ValueStorage: MonoBehaviour
    {
        private object _value;
        public object Value
        {
            get
            {
                this._value = ValueGetDecoration(this._value);
                return this._value;
            }
            set
            {
                this._value = value;
                ValueSetDecoration(this._value);
            }
        }

        public T GetValue<T>() => (T)this.Value;

        abstract protected object ValueGetDecoration(object presentedValue);
        abstract protected void ValueSetDecoration(object newValue);
    }

#if UNITY_EDITOR
    public static class EditorSFX
    {

        public static void PlayClip(AudioClip clip, int startSample = 0, bool loop = false)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                null
            );

            //Debug.Log(method);
            method.Invoke(
                null,
                new object[] { clip, startSample, loop }
            );
        }

        public static void StopAllClips()
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;

            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { },
                null
            );

            //Debug.Log(method);
            method.Invoke(
                null,
                new object[] { }
            );
        }
    }
#endif

    public static class ManagerSpace
    {
        #region Instantiate
        public static UnityEngine.Object InstantiatedProcess(UnityEngine.Object instantiated, IEntityManager entityManagerOwner = null)
        {
            if (entityManagerOwner != null)
            {
                if (instantiated is GameObject)
                {
                    (instantiated as GameObject).GetComponentsInChildren<IManagable>().ForEach(x => (x as IManagable).ownerManagerSpace = entityManagerOwner);
                }
            }
            return instantiated;
        }

        public static T Instantiate<T>(T original, Transform parent, IEntityManager entityManagerOwner = null) where T : UnityEngine.Object
        {
            var instantiated = UnityEngine.Object.Instantiate<T>(original, parent);
            return (T)InstantiatedProcess(instantiated, entityManagerOwner);
        }
        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, IEntityManager entityManagerOwner = null)
        {
            var instantiated = UnityEngine.Object.Instantiate(original, position, rotation);
            return InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays, IEntityManager entityManagerOwner = null) where T : UnityEngine.Object
        {
            var instantiated = UnityEngine.Object.Instantiate<T>(original, parent, worldPositionStays);
            return (T)InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static UnityEngine.Object Instantiate(UnityEngine.Object original, IEntityManager entityManagerOwner = null)
        {
            var instantiated = UnityEngine.Object.Instantiate(original);
            return InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Vector3 position, Quaternion rotation, Transform parent, IEntityManager entityManagerOwner = null)
        {
            var instantiated = UnityEngine.Object.Instantiate(original, position, rotation, parent);
            return InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, bool instantiateInWorldSpace, IEntityManager entityManagerOwner = null)
        {
            var instantiated = UnityEngine.Object.Instantiate(original, parent, instantiateInWorldSpace);
            return InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static T Instantiate<T>(T original, IEntityManager entityManagerOwner = null) where T : UnityEngine.Object
        {
            var instantiated = UnityEngine.Object.Instantiate<T>(original);
            return (T)InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, IEntityManager entityManagerOwner = null) where T : UnityEngine.Object
        {
            var instantiated = UnityEngine.Object.Instantiate<T>(original, position, rotation);
            return (T)InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent, IEntityManager entityManagerOwner = null) where T : UnityEngine.Object
        {
            var instantiated = UnityEngine.Object.Instantiate<T>(original, position, rotation, parent);
            return (T)InstantiatedProcess(instantiated, entityManagerOwner);
        }

        public static UnityEngine.Object Instantiate(UnityEngine.Object original, Transform parent, IEntityManager entityManagerOwner = null)
        {
            var instantiated = UnityEngine.Object.Instantiate(original, parent);
            return InstantiatedProcess(instantiated, entityManagerOwner);
        }
        #endregion
        #region Component

        private static Component ComponentProcess(Component component, IEntityManager entityManagerOwner)
        {
            if (component is IManagable && entityManagerOwner != null)
                (component as IManagable).ownerManagerSpace = entityManagerOwner;
            return component;
        }

        public static UnityEngine.Component AddComponent<T>(this UnityEngine.GameObject gameObject, IEntityManager entityManagerOwner) where T : Component
        {
            return (T)ComponentProcess(gameObject.AddComponent<T>(), entityManagerOwner);
        }
        public static UnityEngine.Component AddComponent(this UnityEngine.GameObject gameObject, Type typeComponent, IEntityManager entityManagerOwner)
        {
            return ComponentProcess(gameObject.AddComponent(typeComponent), entityManagerOwner);
        }
        #endregion
    }

    public static class TaskExts
    {
        public static Task LogExceptionIfFaulted(this Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.IsFaulted || t.Exception != null)
                {
                    ClientInitService.instance.ExecuteInstruction(() => Debug.LogException(t.Exception.Flatten().InnerException));
                }
            });//, TaskScheduler.FromCurrentSynchronizationContext());
            return task;
        }
    }

    public class TaskEx : Task
    {
        public TaskEx(Action action) : base(action)
        {
        }

        public TaskEx(Action action, CancellationToken cancellationToken) : base(action, cancellationToken)
        {
        }

        public TaskEx(Action action, TaskCreationOptions creationOptions) : base(action, creationOptions)
        {
        }

        public TaskEx(Action<object> action, object state) : base(action, state)
        {
        }

        public TaskEx(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, cancellationToken, creationOptions)
        {
        }

        public TaskEx(Action<object> action, object state, CancellationToken cancellationToken) : base(action, state, cancellationToken)
        {
        }

        public TaskEx(Action<object> action, object state, TaskCreationOptions creationOptions) : base(action, state, creationOptions)
        {
        }

        public TaskEx(Action<object> action, object state, CancellationToken cancellationToken, TaskCreationOptions creationOptions) : base(action, state, cancellationToken, creationOptions)
        {
        }

        public static void RunAsync(Action action)
        {
            Func<Task> asyncUpd = async () =>
            {
                await Task.Run(() => {
                    action();
                }).LogExceptionIfFaulted().ConfigureAwait(false);
            };
            asyncUpd();
        }
    }

    public static partial class DateTimeExtensions
    {
        private static long ServerTime;
        private static long LocalTime;

        public static long TicksToMilliseconds(long ticks) => ticks / 10000;
        public static long MillisecondToTicks(long ms) => ms * 10000;
        public static long NowServerTicks => DateTime.Now.Ticks + (ServerTime - LocalTime);

        public static void UpdateServerTime(long ServerTicks)
        {
            ServerTime = ServerTicks;
            LocalTime = DateTime.Now.Ticks;
        }
        private static int DateValue(this DateTime dt)
        {
            return dt.Year * 372 + (dt.Month - 1) * 31 + dt.Day - 1;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2)
        {
            return dt.MonthsBetween(dt2) / 12;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            return dt.MonthsBetween(dt2, includeLastDay) / 12;
        }

        public static int YearsBetween(this DateTime dt, DateTime dt2, bool includeLastDay, out int excessMonths)
        {
            int months = dt.MonthsBetween(dt2, includeLastDay);
            excessMonths = months % 12;
            return months / 12;
        }

        public static int MonthsBetween(this DateTime dt, DateTime dt2)
        {
            int months = (dt2.DateValue() - dt.DateValue()) / 31;
            return Math.Abs(months);
        }

        public static int MonthsBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            if (!includeLastDay) return dt.MonthsBetween(dt2);
            int days;
            if (dt2 >= dt)
                days = dt2.AddDays(1).DateValue() - dt.DateValue();
            else
                days = dt.AddDays(1).DateValue() - dt2.DateValue();
            return days / 31;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2)
        {
            return dt.DaysBetween(dt2) / 7;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            return dt.DaysBetween(dt2, includeLastDay) / 7;
        }

        public static int WeeksBetween(this DateTime dt, DateTime dt2, bool includeLastDay, out int excessDays)
        {
            int days = dt.DaysBetween(dt2, includeLastDay);
            excessDays = days % 7;
            return days / 7;
        }

        public static int DaysBetween(this DateTime dt, DateTime dt2)
        {
            return (dt2.Date - dt.Date).Duration().Days;
        }

        public static int DaysBetween(this DateTime dt, DateTime dt2, bool includeLastDay)
        {
            int days = dt.DaysBetween(dt2);
            if (!includeLastDay) return days;
            return days + 1;
        }
    }

    public static class ColorEx
    {
        public static Color ToColor(this Color color, int HexVal)
        {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return (Color)new Color32(R, G, B, 255);
        }

        public static Color ToColor(this Color color, long lHexVal)
        {
            var HexVal = Convert.ToInt32(lHexVal);
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return (Color)new Color32(R, G, B, 255);
        }

        public static string ToHex(this Color color)
        {
            Color32 c = color;
            var hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", c.r, c.g, c.b, c.a);
            return hex;
        }

        public static Color ToColor(int HexVal)
        {
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return (Color)new Color32(R, G, B, 255);
        }

        public static Color ToColor(long lHexVal)
        {
            var HexVal = Convert.ToInt32(lHexVal);
            byte R = (byte)((HexVal >> 16) & 0xFF);
            byte G = (byte)((HexVal >> 8) & 0xFF);
            byte B = (byte)((HexVal) & 0xFF);
            return new Color32(R, G, B, 255);
        }
    }

    public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private List<TKey> keys = new List<TKey>();
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        private void AddImpl(TKey key, TValue value)
        {
            //lock (dictionary)
            {
                dictionary.Add(key, value);
                keys.Add(key);
            }
        }

        private TValue GetImpl(TKey key)
        {
            //lock (dictionary)
                return dictionary[key];
        }

        public TValue Get(int index)
        {
            //lock (dictionary)
                return dictionary[keys[index]];
        }

        private void SetImpl(TKey key, TValue value)
        {
            //lock (dictionary)
            {
                if (dictionary.ContainsKey(key))
                    dictionary[key] = value;
                else
                {
                    dictionary.Add(key, value);
                    keys.Add(key);
                }
            }
        }

        private bool RemoveImpl(TKey key)
        {
            //lock (dictionary)
            {
                keys.Remove(key);
                return dictionary.Remove(key);
            }
        }

        public bool Remove(int index)
        {
            //lock(dictionary)
            {
                var ret = dictionary.Remove(keys[index]);
                keys.RemoveAt(index);
                return ret;
            }
        }

        private void ClearImpl()
        {
            //lock (dictionary)
            {
                dictionary.Clear();
                keys.Clear();
            }
        }

        public TValue this[TKey key] { get => GetImpl(key); set => SetImpl(key, value); }

        public ICollection<TKey> Keys => keys;

        public ICollection<TValue> Values { 
            get
            {
                List<TValue> values = new List<TValue>();
                keys.ForEach(x => values.Add(dictionary[x]));
                return values;
            } 
        }

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            AddImpl(key, value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            AddImpl(item.Key, item.Value);
        }

        public void Clear()
        {
            ClearImpl();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return dictionary.ContainsKey(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return dictionary.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            return RemoveImpl(key);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveImpl(item.Key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return dictionary.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
    }

    public static class Reflection
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                if (!(targetProperty.CanRead && targetProperty.GetMethod.IsStatic) ||
                (targetProperty.CanWrite && targetProperty.SetMethod.IsStatic))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }
}