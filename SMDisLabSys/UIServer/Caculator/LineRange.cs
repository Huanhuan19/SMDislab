using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Defaults;
using LiveCharts;
using NPOI.SS.Formula.Functions;

namespace SMDisLabSys.UIServer.Caculator
{
    public class LineRange
    {
        public static LineRange Instance = new LineRange();
        public double GetLineMax(ChartValues<ObservablePoint> points, double maxCoe = 1.1)
        {
            var values = (from o in points select o.X).ToList();
            var max = values.Max();
            if (max > 0)
            {
                return max * maxCoe;
            }
            else
            {
                return max * (1 - (maxCoe - 1));
            }
        }
        public double GetLineMin(ChartValues<ObservablePoint> points, double minCoe = 0.9)
        {
            var values = (from o in points select o.X).ToList();
            var min = values.Min();
            if (min > 0)
            {
                return min * minCoe;
            }
            else
            {
                return min * (1 - (minCoe - 1));
            }
        }
    }
}
