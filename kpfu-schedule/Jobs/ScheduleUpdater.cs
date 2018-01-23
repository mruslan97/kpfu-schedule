using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using kpfu_schedule.Models;
using kpfu_schedule.Tools;
using Quartz;
using Telegram.Bot;

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
