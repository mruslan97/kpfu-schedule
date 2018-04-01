using System;
using System.Configuration;
using Quartz;

namespace kpfu_schedule.Jobs
{
    public class WeeksUpdater : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            var currentConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            currentConfig.AppSettings.Settings["WeekNumber"].Value =
                (Convert.ToInt32(currentConfig.AppSettings.Settings["WeekNumber"].Value) + 1).ToString();
            //ConfigurationManager.AppSettings["WeekNumber"] =
            //    (Convert.ToInt32(ConfigurationManager.AppSettings["WeekNumber"]) + 1).ToString();
            currentConfig.AppSettings.Settings["WeekType"].Value =
                currentConfig.AppSettings.Settings["WeekType"].Value.Equals("четная") ? "нечетная" : "четная";
            currentConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}