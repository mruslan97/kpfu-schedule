using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;

namespace kpfu_schedule.Jobs
{
    public class UpdateScheduler
    {
        public static void Start()
        {
            var scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            var job = JobBuilder.Create<ScheduleUpdater>().Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(00,00))
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}