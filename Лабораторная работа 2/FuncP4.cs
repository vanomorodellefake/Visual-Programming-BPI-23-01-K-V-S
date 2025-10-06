using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class FuncP4 : FuncSolver
    {
        private double _a;
        private double _d;
        private double _c;
        public FuncP4(double a, double d, double c)
        {
            _a = a;
            _d = d;
            _c = c;
        }
        public override double Calculate()
        {
            double res = 1;
            for (int i = 0; i < _d; i++)
            {
                res = res * (_c + _a) + 1;
            }
            return res;
        }
    }
}
