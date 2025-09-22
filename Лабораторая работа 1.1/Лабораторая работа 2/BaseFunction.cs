using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторая_работа_2
{
    public abstract class BaseFunction
    {
        public double _Coefficient {  get; set; }
        public double _Znach { get; set; }

        public BaseFunction(double _coefficient)
        {
            _Coefficient = _coefficient;
        }

        public abstract double Calculate(double x);
        public virtual BaseFunction Derivative()
        {
            return null;
        }
    }
}
