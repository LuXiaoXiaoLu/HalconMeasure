using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal class FindData:IResultInfo
    {
        public FindData(bool isOk, double angle, double rowCenter, double colCenter, string workpieceId)
        {
            IsOk = isOk;
            Angle = angle;
            RowCenter = rowCenter;
            ColCenter = colCenter;
            WorkpieceId = workpieceId;
        }
        public FindData()
        {
                
        }

        public List<string> ToList()
        {
            List<string> list = new List<string>();
            list.Add(IsOk? "1" :"0");
            list.Add(Angle.ToString());
            list.Add(RowCenter.ToString());
            list.Add(ColCenter.ToString());
            return list;
        }

        public bool IsOk { get; set; }
        public double Angle { get; set; }
        public double RowCenter { get; set; }     
        public double ColCenter { get; set; }

        public string WorkpieceId {  get; set; }
    }
}
