using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UTanksServer.ECS.Types.Battle;

namespace UTanksServer
{
    public static class MathEx
    {
        public static float Rad2Deg => 360f / ((float)Math.PI * 2);
        public static float RadToDeg(float rad)
        {
            return ((rad * MathEx.Rad2Deg) > 360 ? 360 - ((rad * MathEx.Rad2Deg) - 360) : 360 - (rad * MathEx.Rad2Deg)) - 180f;
        }

        public static Quaternion ToQuaternion(Vector3 v)
        {

            float cy = (float)Math.Cos(v.Z * 0.5);
            float sy = (float)Math.Sin(v.Z * 0.5);
            float cp = (float)Math.Cos(v.Y * 0.5);
            float sp = (float)Math.Sin(v.Y * 0.5);
            float cr = (float)Math.Cos(v.X * 0.5);
            float sr = (float)Math.Sin(v.X * 0.5);

            return new Quaternion
            {
                W = (cr * cp * cy + sr * sp * sy),
                X = (sr * cp * cy - cr * sp * sy),
                Y = (cr * sp * cy + sr * cp * sy),
                Z = (cr * cp * sy - sr * sp * cy)
            };

        }

        public static Vector3S ToEulerAngles(Quaternion q)
        {
            Vector3S angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.x = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }
    }

    public static class ClassEx
    {
        public static string RandomString(this Random random, int countSymbols)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, countSymbols)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
    public static class Reflection
    {

        public static bool TryEnterWriteLockAwaiter(this ReaderWriterLockSlim readerWriterLockSlim, int timeout)
        {
            bool executed = false;
            while (!executed)
            {
                try
                {
                    if (!readerWriterLockSlim.IsWriteLockHeld && readerWriterLockSlim.TryEnterWriteLock(timeout))
                    {
                        executed = true;
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
                catch { }
                
            }
            return false;
        }

        public static bool TryEnterReadLockAwaiter(this ReaderWriterLockSlim readerWriterLockSlim, int timeout)
        {
            bool executed = false;
            while (!executed)
            {
                if (readerWriterLockSlim.TryEnterReadLock(timeout))
                {
                    executed = true;
                    return true;
                }
            }
            return false;
        }

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

    public static class FileEx
    {
        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }
    }

    public static partial class DateTimeExtensions
    {
        private static long ServerTime;
        private static long LocalTime;

        public static long TicksToMilliseconds(long ticks) => ticks / 10000;
        public static long MillisecondToTicks(long ms) => ms * 10000;
        public static float TicksToSeconds(long ticks) => (float)Math.Round(Math.Round((double)ticks / 10000) / 1000, 3);
        public static long NowServerTicks => DateTime.Now.Ticks + (ServerTime-LocalTime);

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
                if(value > 0)
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
                if(base.AutoReset)
                    this.Interval = this.baseInterval; 
                TimerStart = DateTime.Now.Ticks; TimerPaused = 0;});
            //base.AutoReset = true;
            base.Disposed += new EventHandler(this.OnDisposeTimer);
            this.Disposed += new EventHandler(this.OnDisposeTimer);
        }

        public TimerEx(TimerEx oldTimer) : base()
        {
            this.Elapsed += (async (sender, e) => {
                if (!base.AutoReset)
                    this.Interval = this.baseInterval; 
                TimerStart = DateTime.Now.Ticks; TimerPaused = 0; });
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
            if(this.Enabled)
            {
                this.Stop();
                TimerPaused = DateTime.Now.Ticks;
            }
        }

        public void Resume()
        {
            if(TimerPaused != 0 && !this.Enabled)
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
