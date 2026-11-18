using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Measure.Halcon.Demo
{
    internal interface IResultInfo
    {
        bool IsOk { get; set; }

        string WorkpieceId {  get; }

        string ToString();


    }
}
