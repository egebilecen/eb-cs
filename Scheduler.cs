using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace EB_Utility
{
    public class ScheduleItem
    {
        public string               Name            { get; }
        public Action<List<object>> Function        { get; }
        public List<object>         Args            { get; set; }
        public ulong                IntervalMS      { get; set; }
        public DateTime             NextExecuteTime { get; set; }

        public ScheduleItem(string name, ulong intervalMS, Action<List<object>> func, List<object> args)
        {
            Name           = name;
            Function       = func;
            Args           = args;

            UpdateInterval(intervalMS);
        }

        public void UpdateInterval(ulong intervalMS=0)
        {
            if(intervalMS > 0) IntervalMS = intervalMS;
            NextExecuteTime = DateTime.Now.AddMilliseconds(IntervalMS);
        }
    }

    public static class Scheduler
    {
        private static Timer clock;
        private static readonly List<ScheduleItem> scheduleItems = new List<ScheduleItem>();

        public static void Start(ulong intervalMS)
        {
            clock = new Timer
            {
                Interval = intervalMS
            };
            clock.Elapsed += new ElapsedEventHandler(ClockElapsed);
            clock.Start();
        }

        public static void Stop()
        {
            clock.Elapsed -= ClockElapsed;
            clock.Stop();
        }

        public static void AddItem(ScheduleItem item, bool runImmediately=false)
        {
            if(runImmediately)
                item.NextExecuteTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(item.IntervalMS));

            scheduleItems.Add(item);
        }

        public static void RemoveItem(string name)
        { 
            int index = scheduleItems.FindIndex(item => item.Name == name);
            if(index != -1) scheduleItems.RemoveAt(index);
        }

        public static ScheduleItem GetItem(string name)
        {
            return scheduleItems.Find(item => item.Name == name);
        }

        public static IReadOnlyCollection<ScheduleItem> GetItems()
        {
            return scheduleItems.AsReadOnly();
        }

        private static void ClockElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;

            foreach(ScheduleItem item in scheduleItems)
            {
                if(item.IntervalMS != 0
                && now >= item.NextExecuteTime)
                {
                    item.UpdateInterval();

                    try { item.Function(item.Args); }
                    catch(Exception ex) 
                    { 
                        string exceptionMessage = $"Exception occured in ScheduleItem callback function. ScheduleItem: {item.Name}";

                        if(ex is AggregateException aggregateEx)
                        {
                            int i=0;
                            foreach(Exception innerEx in aggregateEx.InnerExceptions)
                                Logger.LogException(innerEx, $"{exceptionMessage}\n(THIS IS AN INNER EXCEPTION, NUMBER {++i})");
                        }
                        else Logger.LogException(ex, exceptionMessage);    
                    }
                }
            }
        }

        public static void Dispose()
        {
            clock?.Dispose();
        }
    }

    public class ScheduleItemAsync
    {
        public string                   Name            { get; }
        public Func<List<object>, Task> Function        { get; }
        public List<object>             Args            { get; set; }
        public ulong                    IntervalMS      { get; set; }
        public DateTime                 NextExecuteTime { get; set; }

        public ScheduleItemAsync(string name, ulong intervalMS, Func<List<object>, Task> func, List<object> args)
        {
            Name           = name;
            Function       = func;
            Args           = args;

            UpdateInterval(intervalMS);
        }

        public void UpdateInterval(ulong intervalMS=0)
        {
            if(intervalMS > 0) IntervalMS = intervalMS;
            NextExecuteTime = DateTime.Now.AddMilliseconds(IntervalMS);
        }
    }

    public static class SchedulerAsync
    {
        private static Timer clock;
        private static readonly List<ScheduleItemAsync> scheduleItems = new List<ScheduleItemAsync>();

        public static void Start(ulong intervalMS)
        {
            clock = new Timer
            {
                Interval = intervalMS
            };
            clock.Elapsed += new ElapsedEventHandler(ClockElapsed);
            clock.Start();
        }

        public static void Stop()
        {
            if(clock == null) return;

            clock.Elapsed -= ClockElapsed;
            clock.Stop();
        }

        public static void AddItem(ScheduleItemAsync item, bool runImmediately=false)
        {
            if(runImmediately)
                item.NextExecuteTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(item.IntervalMS));

            scheduleItems.Add(item);
        }

        public static void RemoveItem(string name)
        { 
            int index = scheduleItems.FindIndex(item => item.Name == name);
            if(index != -1) scheduleItems.RemoveAt(index);
        }

        public static ScheduleItemAsync GetItem(string name)
        {
            return scheduleItems.Find(item => item.Name == name);
        }

        public static IReadOnlyCollection<ScheduleItemAsync> GetItems()
        {
            return scheduleItems.AsReadOnly();
        }

        private static async void ClockElapsed(object sender, ElapsedEventArgs e)
        {
            DateTime now = DateTime.Now;

            foreach(ScheduleItemAsync item in scheduleItems)
            {
                if(item.IntervalMS != 0
                && now >= item.NextExecuteTime)
                {
                    item.UpdateInterval();

                    try { await item.Function(item.Args); }
                    catch(Exception ex)
                    { 
                        string exceptionMessage = $"Exception occured in ScheduleItemAsync callback function. ScheduleItemAsync: {item.Name}";

                        if(ex is AggregateException aggregateEx)
                        {
                            int i=0;
                            foreach(Exception innerEx in aggregateEx.InnerExceptions)
                                Logger.LogException(innerEx, $"{exceptionMessage}\n(THIS IS AN INNER EXCEPTION, NUMBER {++i})");
                        }
                        else Logger.LogException(ex, exceptionMessage);            
                    }
                }
            }
        }

        public static void Dispose()
        {
            clock?.Dispose();
        }
    }
}
