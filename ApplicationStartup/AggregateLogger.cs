using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PubgStatistic.Contracts.Interfaces;

namespace ApplicationStartup
{
    internal class AggregateLogger : ILogger
    {
        private readonly IList<ILogger> _loggers;

        public AggregateLogger(IList<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public async Task LogMessageAsync(string message)
        {
            foreach (var logger in _loggers)
            {
                await logger.LogMessageAsync(message);
            }
        }

        public async Task LogErrorAsync(string message)
        {
            foreach (var logger in _loggers)
            {

                await logger.LogErrorAsync(message);
            }
        }
    }
}
