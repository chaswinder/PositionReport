using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionReport.Data.Model
{
    public class PeriodVolume
    {
        public int Period { get; set; }
        public double Volume { get; set; }

        public int GetStartHour()
        {
            return Period == 1 ? 23 : Period-2;
        }
    }
}
