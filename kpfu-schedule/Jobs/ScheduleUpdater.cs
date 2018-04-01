using System;
using kpfu_schedule.Tools;
using Quartz;

namespace kpfu_schedule.Jobs
{
    public class ScheduleUpdater : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var cache = new Cache();
            cache.Update();
        }
    }
}