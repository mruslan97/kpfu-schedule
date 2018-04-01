﻿using System;
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
            var weekJob = JobBuilder.Create<WeeksUpdater>().Build();
            var weekTrigger = TriggerBuilder.Create()
                .WithIdentity("weekTrigger", "weekGroup")
                .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(DayOfWeek.Monday, 0, 0))
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(15,18))
                .Build();
            scheduler.ScheduleJob(job, trigger);
            //scheduler.ScheduleJob(weekJob,weekTrigger);
        }
    }
}