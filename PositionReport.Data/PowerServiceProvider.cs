using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using global::Common.Logging;

using Services;

using PositionReport.Data.Model;

namespace PositionReport.Data
{
    public interface IPowerServiceProvider
    {
        IEnumerable<PeriodVolume> GetData(DateTime date);
        //Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date);
    }

    public class PowerServiceProvider : IPowerServiceProvider
    {
        private readonly IPowerService powerService;
        private readonly ILog log;

        // parameterless constructor which instantiates its own PowerService instance
        public PowerServiceProvider()
        {
            this.powerService = new PowerService();
            this.log = LogManager.GetLogger<PowerServiceProvider>();
        }

        // constructor allowing an external IPowerService to be injected
        public PowerServiceProvider(IPowerService powerService)
        {
            this.powerService = powerService;
            this.log = LogManager.GetLogger<PowerServiceProvider>();
        }

        // With more time, this would have been an obvious place for more unit test coverage...
        public IEnumerable<PeriodVolume> GetData(DateTime date)
        {
            var trades = this.powerService.GetTrades(date);

            // transform the PowerTrade list into an aggregated resultset by Period
            var data = trades
                .SelectMany(t => t.Periods)
                .GroupBy(p => p.Period)
                .Select(v => new PeriodVolume { Period = v.Key, Volume = v.Sum(t => t.Volume) });

            return data;
        }

        //public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        //{
        //    return await this.powerService.GetTradesAsync(date);
        //}
    }
}
