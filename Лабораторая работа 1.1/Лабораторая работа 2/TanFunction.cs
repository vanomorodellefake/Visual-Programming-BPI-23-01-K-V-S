using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторая_работа_2
{
    public class TanFunction: BaseFunction
    {
        public TanFunction(double _coefficient) : base(_coefficient) { }
        public override double Calculate(double x)
        {
            return _Coefficient * Math.Tan(x);
        }
    }
}
