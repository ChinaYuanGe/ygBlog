using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Useful.Tools
{
    public class Timestamp
    {
        public readonly static DateTime Zero = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public DateTime UTCTime { get; set; }
        public DateTime LocalTime => UTCTime.ToLocalTime();
        public long timestamp
        {
            get
            {
                return (UTCTime.Ticks - Zero.Ticks) / TimeSpan.TicksPerSecond;
            }
        }
        public long timestamp_mill
        {
            get {
                return (UTCTime.Ticks - Zero.Ticks) / TimeSpan.TicksPerMillisecond;
            }
        }
        /// <summary>
        /// 根据给出的时间生成一个实例
        /// </summary>
        /// <param name="time"></param>
        public Timestamp(DateTime time) {
            if (time.Kind != DateTimeKind.Utc)
                this.UTCTime = time.ToUniversalTime();
            else
                this.UTCTime = time;
        }
        /// <summary>
        /// 以当前UTC+0的时间生成一个实例
        /// </summary>
        public Timestamp() {
            UTCTime = DateTime.Now.ToUniversalTime();
        }
        public static Timestamp Now { get { return new Timestamp(); } }
        /// <summary>
        /// 以unix时间戳生成一个实例,单位为毫秒
        /// </summary>
        /// <param name="timestamp"></param>
        public Timestamp(long timestamp_mill) {
            UTCTime = Zero.AddMilliseconds(timestamp_mill);
        }
        /// <summary>
        /// 以unix时间戳生成一个实例,单位为秒
        /// </summary>
        /// <param name="timestamp"></param>
        public Timestamp(int timestamp) {
            UTCTime = Zero.AddSeconds(timestamp);
        }
        public static Timestamp operator +(Timestamp left, Timestamp right) => new Timestamp(left.timestamp_mill + right.timestamp_mill);
        public static Timestamp operator +(Timestamp left, long right) => new Timestamp(left.timestamp_mill + right);
        public static Timestamp operator -(Timestamp left, Timestamp right) => new Timestamp(left.timestamp_mill - right.timestamp_mill);
        public static Timestamp operator -(Timestamp left, long right) => new Timestamp(left.timestamp_mill - right);
        public static bool operator >(Timestamp left, Timestamp right) => left.timestamp_mill > right.timestamp_mill;
        public static bool operator >=(Timestamp left, Timestamp right) => left.timestamp_mill >= right.timestamp_mill;

        public static bool operator <(Timestamp left, Timestamp right) => left.timestamp_mill < right.timestamp_mill;
        public static bool operator <=(Timestamp left, Timestamp right) => left.timestamp_mill <= right.timestamp_mill;
        public static bool operator ==(Timestamp left, Timestamp right) => left.timestamp_mill == right.timestamp_mill;
        public static bool operator !=(Timestamp left, Timestamp right) => left.timestamp_mill != right.timestamp_mill;


    }
}
