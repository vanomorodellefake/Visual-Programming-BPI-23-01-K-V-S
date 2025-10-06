using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class FuncP2 : FuncSolver
    {
        private double _a;
        private double _b;
        private double _f;
        public FuncP2(double a, double b, double f)
        {
            _a = a;
            _b = b;
            _f = f;
        }
        public override double Calculate()
        {
            return Math.Cos(_f * _a) + Math.Sin(_f * _b);
        }
    }
}
