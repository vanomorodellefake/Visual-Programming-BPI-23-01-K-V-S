using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Лабораторая_работа_2
{
    public class SinFunction: BaseFunction
    {
        public SinFunction(double _coefficient) : base(_coefficient) { }
        public override double Calculate(double x)
        {
            return _Coefficient * Math.Sin(x);
        }
        public override BaseFunction Derivative()
        {
            return new CosFunction(_Coefficient);
        }
    }
}
