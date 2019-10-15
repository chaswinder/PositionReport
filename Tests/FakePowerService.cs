using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Services;

namespace TestProject
{
    public class FakePowerService : IPowerService
    {
        private readonly IEnumerable<PowerTrade> fakePowerTrades;

        public FakePowerService(IEnumerable<PowerTrade> powerTrades)
        {
            this.fakePowerTrades = powerTrades;
        }

        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            return this.fakePowerTrades;
        }

        public Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
