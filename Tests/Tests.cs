using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Services;

using PositionReport.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    [TestClass]
    public class Tests
    {        
        [TestMethod]
        public void Test_ScenarioFromPDF_Passes()
        {
            // Explicitly run the test data scenario given in the PDF
            var theDate = DateTime.Parse("01/04/2015");

            var testData = SetupTestData();

            var fakePowerService = new FakePowerService(testData);

            var provider = new PowerServiceProvider(fakePowerService);

            var output = provider.GetData(theDate);

            Assert.AreEqual(output.Count(), 24);
            foreach (var line in output)
            {
                if (line.Period >= 0 && line.Period <= 11)
                {
                    Assert.AreEqual(line.Volume, 150.0);
                }
                else
                {
                    Assert.AreEqual(line.Volume, 80);
                }
            }
        }

        #region Helper methods

        private PowerTrade[] SetupTestData()
        {
            var theDate = DateTime.Parse("01/04/2015");

            var volumes1 = new[] { 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 
                                    100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 
                                    100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0, 100.0 };

            var volumes2 = new[] { 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0, 50.0,
                                    50.0, 50.0, 50.0, -20.0, -20.0, -20.0, -20.0, -20.0,
                                   -20.0, -20.0, -20.0, -20.0, -20.0, -20.0, -20.0, -20.0 };

            var trades = new[]
            {
                GetTrade(theDate, 24, volumes1),
                GetTrade(theDate, 24, volumes2)
            };

            return trades;
        }

        private PowerTrade GetTrade(DateTime theDate, int numPeriods, double[] volumes)
        {
            var trade = PowerTrade.Create(theDate, 24);

            for (int i = 1; i <= 24; i++)
            {
                var period = trade.Periods[i-1];
                period.Volume = volumes[i-1];
            }

            return trade;
        }

        #endregion
    }
}
