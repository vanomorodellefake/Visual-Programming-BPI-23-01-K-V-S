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

namespace Лабораторая_работа_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BaseFunction _BaseFunction;

        private double _Coefficient;
        private double _ZnachPerem;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReturnZnach(object sender, RoutedEventArgs e)
        {
            if (!CheckZero()) { return; }

            CreateCurrentFunction();

            if (_BaseFunction != null)
            {
                double result = _BaseFunction.Calculate(_ZnachPerem);
                MessageBox.Show($"f({_ZnachPerem}) = {result:F4}", "Значение функции");
            }
        }

        private void CreateCurrentFunction()
        {
            int selectedIndex = ChooseOfFunction.SelectedIndex;
            switch (selectedIndex)
            {
                case 1:
                    _BaseFunction = new SinFunction(_Coefficient);
                    //MessageBox.Show(Convert.ToString(sinFunction.Calculate(_ZnachPerem)), "Значение функции");
                    break;

                case 2:
                    _BaseFunction = new CosFunction(_Coefficient);
                    //MessageBox.Show(Convert.ToString(cosFunction.Calculate(_ZnachPerem)), "Значение функции");
                    break;

                case 3:
                    _BaseFunction = new TanFunction(_Coefficient);
                    //MessageBox.Show(Convert.ToString(tanFunction.Calculate(_ZnachPerem)), "Значение функции");
                    break;
            }
        }

        private void CreateDerivative(object sender, RoutedEventArgs e)
        {
            if (!CheckZero()) { return; }

            CreateCurrentFunction();

            if (_BaseFunction != null)
            {
                _BaseFunction = _BaseFunction.Derivative();
                double result = _BaseFunction.Calculate(_ZnachPerem);
                MessageBox.Show($"f({_ZnachPerem}) = {result:F4}", "Значение производной функции");
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_Checker(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Decimal && e.Key != Key.OemPeriod && e.Key !=  Key.OemComma)
            {
                e.Handled = true;
            }
        }
        private bool CheckZero()
        {
            bool SelectionError = false;
            bool DataError = false;

            if (ChooseOfFunction.SelectedIndex <= 0)
            {
                SelectionError = true;
            }
            try
            {
                UpdateData();
            }
            catch
            {
                DataError = true;
            }
            if (SelectionError && DataError)    
            {
                MessageBox.Show("Было найдено пустое поле и не выбрана функция", "Ошибка!");
                return false;
            }
            else if (SelectionError) 
            {
                MessageBox.Show("Не выбрана функция", "Ошибка!");
                return false;
            }
            else if (DataError)
            {
                MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                return false;
            }

            return true;
        }
        private void UpdateData()
        {
            _Coefficient = Convert.ToDouble(Coefficient.Text);
            _ZnachPerem = Convert.ToDouble(znach_peremen.Text);

            //MessageBox.Show($"{_Coefficient} = {_ZnachPerem}", "Тест");
        }
    }
}
