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
        private double A;
        private double F;
        public FuncP1(double a, double f)
        {
            A = a;
            F = f;
        }
        public override double Calculate()
        {
            return Math.Sin(F * A);
        }
    }
    public class FuncP2 : FuncSolver
    {
        private double A;
        private double B;
        private double F;
        public FuncP2(double a, double b, double f)
        {
            A = a;
            B = b;
            F = f;
        }
        public override double Calculate()
        {
            return Math.Cos(F*A) + Math.Sin(F * B);
        }
    }
    public class FuncP3 : FuncSolver
    {
        private double A;
        private double B;
        private double C;
        private double D;
        public FuncP3(double a, double b, double c, double d)
        {
            A = a;
            B = b;
            C = c;
            D = d;
        }
        public override double Calculate()
        {
            return C*A*A + D*B*B;
        }
    }
    public class FuncP4 : FuncSolver
    {
        private double A;
        private double D;
        private double C;
        public FuncP4(double a, double d, double c)
        {
            A = a;
            D = d;
            C = c;
        }
        public override double Calculate()
        {
            double res = 1;
            for (int i = 0; i<D; i++)
            {
                res = res * (C + A) + 1;
            }
            return res;
        }
    }
}
