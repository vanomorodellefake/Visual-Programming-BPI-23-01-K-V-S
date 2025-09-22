using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Лабораторная_работа_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FuncSolver funcSolver;
        public int _currentRadiobuttonIndex = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentRadiobuttonIndex)
            {
                case 0:
                    double p1a = Convert.ToDouble(R1TextA.Text);
                    double p1f = Convert.ToDouble(R1ComboF.Text);
                    funcSolver = new FuncP1(p1a, p1f);
                    this.Title = "Ответ: " + funcSolver.Calculate().ToString();
                    break;
                case 1:
                    double p2a = Convert.ToDouble(R2TextA.Text);
                    double p2b = Convert.ToDouble(R2TextB.Text);
                    double p2f = Convert.ToDouble(R2ComboF.Text);
                    funcSolver = new FuncP2(p2a, p2b, p2f);
                    this.Title = "Ответ: " + funcSolver.Calculate().ToString();
                    break;
                case 2:
                    double p3a = Convert.ToDouble(R3TextA.Text);
                    double p3b = Convert.ToDouble(R3TextB.Text);
                    double p3c = Convert.ToDouble(R3ComboC.Text);
                    double p3d = Convert.ToDouble(R3ComboD.Text);
                    funcSolver = new FuncP3(p3a,p3b, p3c, p3d);
                    this.Title = "Ответ: " + funcSolver.Calculate().ToString();
                    break;
                case 3:
                    double p4a = Convert.ToDouble(R4TextA.Text);
                    double p4d = Convert.ToDouble(R4TextD.Text);
                    double p4c = Convert.ToDouble(R4ComboC.Text);
                    funcSolver = new FuncP4(p4a, p4d, p4c);
                    this.Title = "Ответ: " + funcSolver.Calculate().ToString();
                    break;
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (Radio1.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 0;
                return;
            }

            if (Radio2.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 1;
                return;
            }

            if (Radio3.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 2;
                return;
            }

            if (Radio4.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 3;
                return;
            }
        }

        private void TextBox_Checker(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Decimal && e.Key != Key.OemPeriod && e.Key != Key.OemComma)
            {
                e.Handled = true;
            }
        }
    }
}
