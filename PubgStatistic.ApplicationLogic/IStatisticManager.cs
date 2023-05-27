using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubgStatistic.ApplicationLogic
{
    public interface IStatisticManager
    {
        Task StartNewGameSessionAsync(CancellationToken ct = default);
    }
}
