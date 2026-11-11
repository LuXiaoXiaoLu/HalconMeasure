using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal class MeasureData
    {
        public bool IsOk { get; set; }
        public double DiameterMm { get; set; }
        public double RowCenter { get; set; }     // 圆心行
        public double ColCenter { get; set; }
    }
}
