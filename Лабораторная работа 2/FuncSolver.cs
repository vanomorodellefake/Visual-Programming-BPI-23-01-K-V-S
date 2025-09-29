using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public abstract class FuncSolver
    {
        public abstract double Calculate();
    }

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
            return Math.Cos(_f*_a) + Math.Sin(_f * _b);
        }
    }
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
            return _c*_a*_a + _d*_b*_b;
        }
    }
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
            for (int i = 0; i<_d; i++)
            {
                res = res * (_c + _a) + 1;
            }
            return res;
        }
    }

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
            for (int i = 1; i<=_n; i++)
            {
                for (int j = 1; j<=_k; j++)
                {
                    res += ( Math.Cos(Math.Pow(_x, j)) + j ) / (i * j);
                }
            }
            return res;
        }
    }
}
