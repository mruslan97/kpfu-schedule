using Quartz;
using Quartz.Impl;

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
                .StartNow()
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(04, 14))
                .Build();
            scheduler.ScheduleJob(job, trigger);
        }
    }
}