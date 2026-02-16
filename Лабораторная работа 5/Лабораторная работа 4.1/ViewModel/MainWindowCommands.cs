using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.Helper;
using WpfApp.View;

namespace WpfApp.ViewModel
{
    public class MainWindowCommands
    {
        private RelayCommand employeeOpen;
        public RelayCommand EmployeeOpen
        {
            get
            {
                return employeeOpen ??
                (employeeOpen = new RelayCommand(obj =>
               {
                   WindowsEmployee wEmployee = new WindowsEmployee();
                   wEmployee.ShowDialog();
               }));
            }
        }
        private RelayCommand roleOpen;
        public RelayCommand RoleOpen
        {
            get
            {
                return roleOpen ??
                (roleOpen = new RelayCommand(obj =>
                {
                    WindowsRole wRole = new WindowsRole();
                    wRole.ShowDialog();
                }));
            }
        }
    }
}
