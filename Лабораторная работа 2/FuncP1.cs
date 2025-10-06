using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class FuncP1 : FuncSolver
    {
        private double _a;
        private double _f;
        public FuncP1(double a, double f)
        {
            _a = a;
            _f = f;
        }
        public override double Calculate()
        {
            return Math.Sin(_f * _a);
        }
    }
}
