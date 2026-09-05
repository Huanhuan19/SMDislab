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
        public double GetLineMax(ChartValues<ObservablePoint> points, double maxCoe = 0.03)
        {
            var values = (from o in points select o.X).ToList();
            var min = values.Min();
            var max = values.Max();
            var range = max - min;
            if (range == 0)//仅一个点时  
            {
                return max + 0.1;
            }
            else
            {
                var distance = range * maxCoe;
                return max + distance;
            }
        }
        public double GetLineMin(ChartValues<ObservablePoint> points, double minCoe = 0.03)
        {
            var values = (from o in points select o.X).ToList();
            var min = values.Min();
            var max = values.Max();
            var range = max - min;
            if (range == 0)//仅一个点时  
            {
                return min - 0.1;
            }
            else
            {
                var distance = range * minCoe;
                return min - distance;
            }
        }
        public double GetLineMaxY(ChartValues<ObservablePoint> points, double maxCoe = 0.03)
        {
            var values = (from o in points select o.Y).ToList();
            var min = values.Min();
            var max = values.Max();
            var range = max - min;
            if (range == 0)//仅一个点时  
            {
                return max + 0.1;
            }
            else
            {
                var distance = range * maxCoe;
                return max + distance;
            }
        }
        public double GetLineMinY(ChartValues<ObservablePoint> points, double minCoe = 0.03)
        {
            var values = (from o in points select o.Y).ToList();
            var min = values.Min();
            var max = values.Max();
            var range = max - min;
            if (range == 0)//仅一个点时  
            {
                return min - 0.1;
            }
            else
            {
                var distance = range * minCoe;
                return min - distance;
            }
        }
    }
}
