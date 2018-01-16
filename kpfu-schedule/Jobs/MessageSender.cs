using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Telegram.Bot;

namespace kpfu_schedule.Jobs
{
    public class MessageSender : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("HelloJob is executing.");
        }
    }
}
