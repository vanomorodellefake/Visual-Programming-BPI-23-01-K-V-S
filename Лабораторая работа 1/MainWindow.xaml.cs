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

namespace Лабораторая_работа_1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataSystem dataSystem { get; set; }

        private int Hours;
        private int Minutes;
        private int Seconds;
        private int SecondsToAdd;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReturnSeconds(object sender, RoutedEventArgs e)
        {
            if (CheckZero())
            {

                dataSystem = new DataSystem(Hours, Minutes, Seconds, SecondsToAdd);
                MessageBox.Show(Convert.ToString(dataSystem.ReturnSeconds(dataSystem._Hour, dataSystem._Minutes, dataSystem._Seconds)), "Количество секунд во времени");
            } 
        }

        private void AddSomeSeconds(object sender, RoutedEventArgs e)
        {
            if (CheckZero())
            {
                dataSystem = new DataSystem(Hours, Minutes, Seconds, SecondsToAdd);
                (int, int, int) NewTime = dataSystem.AddSomeSeconds(dataSystem._Hour, dataSystem._Minutes, dataSystem._Seconds, dataSystem._SecondsToAdd);

                MessageBox.Show($"{NewTime.Item1} : {NewTime.Item2} : {NewTime.Item3}", "Время после добавления секунд");
                //hours.Text = Convert.ToString(NewTime.Item1);
                //minutes.Text = Convert.ToString(NewTime.Item2);
                //seconds.Text = Convert.ToString(NewTime.Item3);
            }
        }
        private void TextBox_Checker(object sender, KeyEventArgs e)
        {
            //e.Handled = true;
            //MessageBox.Show("abc", "bca");
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down) 
            {
                //MessageBox.Show("abc", "bca");
                e.Handled = true;
            }
        }
        private bool CheckZero()
        {
            try
            {
                UpdateData();
            }
            catch
            {
                MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                return false;
            }
            return true;
        }

        private void UpdateData()
        {
            Hours = Convert.ToInt32(hours.Text);
            Minutes = Convert.ToInt32(minutes.Text);
            Seconds = Convert.ToInt32(seconds.Text);
            SecondsToAdd = Convert.ToInt32(AmountSecondsToAdd.Text);
        }

        private void TextBox_CheckEntryTime(object sender, TextCompositionEventArgs e)
        {
            var Hours = Convert.ToInt32(hours.Text);
            var Minutes = Convert.ToInt32(minutes.Text);
            var Seconds = Convert.ToInt32(seconds.Text);
            var SecondsToAdd = Convert.ToInt32(AmountSecondsToAdd.Text);

            dataSystem = new DataSystem(Hours, Minutes, Seconds, SecondsToAdd);
            var NewTime = dataSystem.AddSomeSeconds(dataSystem._Hour, dataSystem._Minutes, dataSystem._Seconds, 0);

            hours.Text = Convert.ToString(NewTime.Item1);
            minutes.Text = Convert.ToString(NewTime.Item2);
            seconds.Text = Convert.ToString(NewTime.Item3);
        }
    }
}
