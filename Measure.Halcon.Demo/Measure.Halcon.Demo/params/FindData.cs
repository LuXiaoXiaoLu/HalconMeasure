using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal class FindData
    {
        public FindData(bool isOk, double angle, double rowCenter, double colCenter)
        {
            IsOk = isOk;
            Angle = angle;
            RowCenter = rowCenter;
            ColCenter = colCenter;
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

        
    }
}
