using System;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;

namespace GameMain
{
    /// <summary>
    /// Synchronize time between the server and the clients
    /// </summary>
    public static class NetworkTime
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(NetworkTime));

        /// <summary>
        /// how often are we sending ping messages
        /// used to calculate network time and RTT
        /// </summary>
        public static float PingFrequency = 2.0f;

        /// <summary>
        /// average out the last few results from Ping
        /// </summary>
        public static int PingWindowSize = 10;

        static double lastPingTime;

        // Date and time when the application started
        static readonly Stopwatch stopwatch = new Stopwatch();

        static NetworkTime()
        {
            stopwatch.Start();
        }

        static ExponentialMovingAverage _rtt = new ExponentialMovingAverage(10);
        static ExponentialMovingAverage _offset = new ExponentialMovingAverage(10);

        // the true offset guaranteed to be in this range
        static double offsetMin = double.MinValue;
        static double offsetMax = double.MaxValue;

        // returns the clock time _in this system_
        static double LocalTime()
        {
            return stopwatch.Elapsed.TotalSeconds;
        }

        public static void Reset()
        {
            _rtt = new ExponentialMovingAverage(PingWindowSize);
            _offset = new ExponentialMovingAverage(PingWindowSize);
            offsetMin = double.MinValue;
            offsetMax = double.MaxValue;
        }

        internal static void UpdateClient()
        {
            if (Time.time - lastPingTime >= PingFrequency)
            {
                // NetworkPingMessage pingMessage = new NetworkPingMessage(LocalTime());
                // NetworkClient.Send(pingMessage, Channels.DefaultUnreliable);
                lastPingTime = Time.time;
            }
        }

        /// <summary>
        /// The time in seconds since the server started.
        /// </summary>
        //
        // I measured the accuracy of float and I got this:
        // for the same day,  accuracy is better than 1 ms
        // after 1 day,  accuracy goes down to 7 ms
        // after 10 days, accuracy is 61 ms
        // after 30 days , accuracy is 238 ms
        // after 60 days, accuracy is 454 ms
        // in other words,  if the server is running for 2 months,
        // and you cast down to float,  then the time will jump in 0.4s intervals.
        public static double time => LocalTime() - _offset.Value;

        /// <summary>
        /// Measurement of the variance of time.
        /// <para>The higher the variance, the less accurate the time is</para>
        /// </summary>
        public static double timeVar => _offset.Var;

        /// <summary>
        /// standard deviation of time.
        /// <para>The higher the variance, the less accurate the time is</para>
        /// </summary>
        public static double timeSd => Math.Sqrt(timeVar);

        /// <summary>
        /// Clock difference in seconds between the client and the server
        /// </summary>
        /// <remarks>
        /// Note this value is always 0 at the server
        /// </remarks>
        public static double offset => _offset.Value;

        /// <summary>
        /// how long in seconds does it take for a message to go
        /// to the server and come back
        /// </summary>
        public static double rtt => _rtt.Value;

        /// <summary>
        /// measure variance of rtt
        /// the higher the number,  the less accurate rtt is
        /// </summary>
        public static double rttVar => _rtt.Var;

        /// <summary>
        /// Measure the standard deviation of rtt
        /// the higher the number,  the less accurate rtt is
        /// </summary>
        public static double rttSd => Math.Sqrt(rttVar);
    }
}
