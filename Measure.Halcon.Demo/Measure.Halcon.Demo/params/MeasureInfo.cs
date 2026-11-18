using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal class MeasureInfo:IResultInfo
    {
        public MeasureInfo(bool isOk, double diameterMm, double rowCenter, double colCenter, string workpieceId)
        {
            IsOk = isOk;
            DiameterMm = diameterMm;
            RowCenter = rowCenter;
            ColCenter = colCenter;
            WorkpieceId = workpieceId;
        }

        public MeasureInfo()
        {
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[R,{0},{1},{2},{3},{4}]",
                WorkpieceId,
                IsOk ? "1" : "0",
                DiameterMm, RowCenter, ColCenter);
        }

        
        public bool IsOk { get; set; }
        public double DiameterMm { get; set; }
        public double RowCenter { get; set; }     // 圆心行
        public double ColCenter { get; set; }

        public string WorkpieceId {  get; set; }
    }
}
