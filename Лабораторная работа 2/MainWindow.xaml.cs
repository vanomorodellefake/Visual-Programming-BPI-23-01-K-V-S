using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
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
using Лабораторная_работа_2.Properties;

namespace Лабораторная_работа_2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FuncSolver _funcSolver;
        private int _currentRadiobuttonIndex = 0;

        int k_style = 0;
        public MainWindow()
        {
            InitializeComponent();

            ImageP1.Source = BitMapToBitMapImageConverter(Properties.Resources.p1);
            ImageP2.Source = BitMapToBitMapImageConverter(Properties.Resources.p2);
            ImageP3.Source = BitMapToBitMapImageConverter(Properties.Resources.p3);
            ImageP4.Source = BitMapToBitMapImageConverter(Properties.Resources.p4);
            ImageP5.Source = BitMapToBitMapImageConverter(Properties.Resources.p5);
        }

        private BitmapImage BitMapToBitMapImageConverter(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void DisableAllPanels()
        {
            if (PanelP1 == null) return;
            if (PanelP2 == null) return;
            if (PanelP3 == null) return;
            if (PanelP4 == null) return;
            if (PanelP5 == null) return;

            PanelP1.IsEnabled = false;
            PanelP2.IsEnabled = false;
            PanelP3.IsEnabled = false;
            PanelP4.IsEnabled = false;
            PanelP5.IsEnabled = false;
        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentRadiobuttonIndex)
            {
                case 0:
                    try
                    {
                        double p1a = Convert.ToDouble(R1TextA.Text);
                        double p1f = Convert.ToDouble(R1ComboF.Text);
                        _funcSolver = new FuncP1(p1a, p1f);
                        this.Title = "Ответ: " + _funcSolver.Calculate().ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                        break;
                    }
                    break;
                case 1:
                    try
                    {
                        double p2a = Convert.ToDouble(R2TextA.Text);
                        double p2b = Convert.ToDouble(R2TextB.Text);
                        double p2f = Convert.ToDouble(R2ComboF.Text);
                        _funcSolver = new FuncP2(p2a, p2b, p2f);
                        this.Title = "Ответ: " + _funcSolver.Calculate().ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                        break;
                    }
                    break;
                case 2: 
                    try
                    {
                        double p3a = Convert.ToDouble(R3TextA.Text);
                        double p3b = Convert.ToDouble(R3TextB.Text);
                        double p3c = Convert.ToDouble(R3ComboC.Text);
                        double p3d = Convert.ToDouble(R3ComboD.Text);
                        _funcSolver = new FuncP3(p3a, p3b, p3c, p3d);
                        this.Title = "Ответ: " + _funcSolver.Calculate().ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                        break;
                    }
                    break;
                case 3:
                    try
                    {
                        double p4a = Convert.ToDouble(R4TextA.Text);
                        double p4d = Convert.ToDouble(R4TextD.Text);
                        double p4c = Convert.ToDouble(R4ComboC.Text);
                        _funcSolver = new FuncP4(p4a, p4d, p4c);
                        this.Title = "Ответ: " + _funcSolver.Calculate().ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                        break;
                    }
                    break;
                case 4:
                    try
                    {
                        int p5n = Convert.ToInt32(R5TextN.Text);
                        int p5k = Convert.ToInt32(R5TextK.Text);
                        double p5x = Convert.ToDouble(R5TextX.Text);
                        _funcSolver = new FuncP5(p5n, p5k, p5x);
                        this.Title = "Ответ: " + _funcSolver.Calculate().ToString();
                    }
                    catch
                    {
                        MessageBox.Show("Было найдено пустое поле", "Ошибка!");
                        break;
                    }               
                    break;
            }
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            DisableAllPanels();
            if (Radio1.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 0;
                if (PanelP1 != null)
                    PanelP1.IsEnabled = true;
                return;
            }

            else if (Radio2.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 1;
                if (PanelP2 != null)
                    PanelP2.IsEnabled = true;
                return;
            }

            else if(Radio3.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 2;
                if (PanelP3 != null)
                    PanelP3.IsEnabled = true;
                return;
            }

            else if(Radio4.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 3;
                if (PanelP4 != null)
                    PanelP4.IsEnabled = true;
                return;
            }

            else if(Radio5.IsChecked.GetValueOrDefault())
            {
                _currentRadiobuttonIndex = 4;
                if (PanelP5 != null)
                    PanelP5.IsEnabled = true;
                return;
            }
        }

        private void TextBox_TextChecker(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ",")
            {
                var __ = (TextBox)sender;
                if (__.Text.Contains(","))
                {
                    e.Handled = true;
                    return;
                }
            }
            try
            {
                
                var _ = Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void TextBox_TextChecker_int(object sender, TextCompositionEventArgs e)
        {
            try
            {
                var _ = Convert.ToInt32(e.Text);
            }
            catch
            {
                e.Handled = true;
            }
        }

        private void TextBox_Checker_Text(object sender, TextCompositionEventArgs e)
        {
            var txt = sender as TextBox;
            if ((!char.IsDigit(e.Text, 0)) && (e.Text != ","))
            {
                e.Handled = true;
            }
            else if ((e.Text == ",") && ((txt.Text.IndexOf(",") != -1) || (txt.Text == "")))
            {
                e.Handled = true;
            }
        }
        private void TextBox_Checker_Text_int(object sender, TextCompositionEventArgs e)
        {
            var txt = sender as TextBox;
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void TextBox_Checker(object sender, KeyEventArgs e)
        {
            var txt = sender as TextBox;
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down && e.Key != Key.Decimal && e.Key != Key.OemPeriod && e.Key != Key.OemComma)
            {
                e.Handled = true;
            }
        }

        private void TextBox_Checker_int(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 && e.Key != Key.Back && e.Key != Key.Left && e.Key != Key.Right && e.Key != Key.Up && e.Key != Key.Down)
            {
                e.Handled = true;
            }
        }

        private void ChangeTheme_Click()
        {
            string style = "";
            k_style++;
            if (k_style % 2 == 0)
            {
                style = "Coffee";
            }
            else
            {
                style = "White";
            }
            // определяем путь к файлу ресурсов
            var uri = new Uri(style + ".xaml", UriKind.Relative);
            // загружаем словарь ресурсов
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            // очищаем коллекцию ресурсов приложения
            Application.Current.Resources.Clear();
            // добавляем загруженный словарь ресурсов
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
