using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal class FindInfo:IResultInfo
    {
        public FindInfo(bool isOk, double angle, double rowCenter, double colCenter, string workpieceId)
        {
            IsOk = isOk;
            Angle = angle;
            RowCenter = rowCenter;
            ColCenter = colCenter;
            WorkpieceId = workpieceId;
        }
        public FindInfo()
        {
                
        }

        public override string  ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "[M,{0},{1},{2},{3},{4}]",
                WorkpieceId,
                IsOk ? "1" : "0",
                Angle, RowCenter, ColCenter);
        }

        public bool IsOk { get; set; }
        public double Angle { get; set; }
        public double RowCenter { get; set; }     
        public double ColCenter { get; set; }

        public string WorkpieceId {  get; set; }
    }
}
