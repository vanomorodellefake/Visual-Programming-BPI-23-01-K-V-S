using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using WpfApp.Helper;
using WpfApp.Model;
using WpfApp.ViewModel;

namespace WpfApp.View
{
    public partial class WindowsEmployee : Window
    {
        public WindowsEmployee()
        {
            InitializeComponent();
            DataContext = new PersonViewModel();
        }
    }
}
