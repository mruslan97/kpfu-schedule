using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotHost.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;

namespace BotHost.Quartz
{
    public class ScheduledJob : IJob
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ScheduledJob> _logger;
        private Cache _cache;

        public ScheduledJob(IConfiguration configuration, ILogger<ScheduledJob> logger, Cache cache)
        {
            _logger = logger;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation($"Starting update quartz task");
            await _cache.Update();
            await Task.CompletedTask;
        }
    }
}
