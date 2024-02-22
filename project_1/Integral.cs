using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;

namespace project_1
{
    public class Integral : IDataErrorInfo
    {
        double a, b;
        int n;

        public Func<double, double> Func = (x) => Math.Cos(2 * x);

        public Integral()
        {
        }

        public Integral(double a, double b, int n)
        {
            A = a;
            B = b;
            N = n;
        }

        public double A { get => a; set => a = value; }
        public double B { get => b; set => b = value; }
        public int N { get => n; set => n = value; }

        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch(columnName)
                {
                    case "A":
                    {
                        if (A < -1 || A > 1)
                        {
                           error = "Начало диапозона должно быть [-1;1]";
                        }
                    }break;

                    case "B":
                    {
                        if (B < 0 || B > Math.PI)
                        {
                           error = "Конец диапозона должно быть [0;Pi]";
                        }
                     }break;

                    case "N":
                    {
                        if (N < 100)
                        {
                           error = "Диапозон значений должен начинаться от 100 больше";
                        }
                     }break;    
                }

                return error;
            }
        }

        public async IAsyncEnumerable<(double,double,int)> GetDoublesAsync()
        {

            double h = (B - A) / N;
            double S = 0;
            for (int i = 0; i < n; i++)
            {
                double x = A + h * i;
                S += Func(x) * h;
                await Task.Delay(50);

                yield return (x,S,100*i/N);
            }
        }
        
        public override string? ToString()
        {
            return $"A={A:0.000}, B={B:0.000}, N={N}";
        }
    }
}
