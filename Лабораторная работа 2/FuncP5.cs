using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class FuncP5 : FuncSolver
    {
        private int _n;
        private int _k;
        private double _x;

        public FuncP5(int n, int k, double x)
        {
            _n = n;
            _k = k;
            _x = x;
        }
        public override double Calculate()
        {
            double res = 0;
            for (int i = 1; i <= _n; i++)
            {
                for (int j = 1; j <= _k; j++)
                {
                    res += (Math.Cos(Math.Pow(_x, j)) + j) / (i * j);
                }
            }
            return res;
        }
    }
}
