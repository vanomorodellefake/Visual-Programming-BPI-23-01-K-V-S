using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class FuncP3 : FuncSolver
    {
        private double _a;
        private double _b;
        private double _c;
        private double _d;
        public FuncP3(double a, double b, double c, double d)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
        }
        public override double Calculate()
        {
            return _c * _a * _a + _d * _b * _b;
        }
    }
}
