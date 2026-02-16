using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp.ViewModel;

namespace WpfApp.Model
{
    public class Person
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Birthday { get; set; }
        public Person() { }
        public Person(int id, int roleId, string firstName, string lastName, string birthday)
        {
            this.Id = id; this.RoleId = roleId;
            this.FirstName = firstName; this.LastName = lastName; this.Birthday = birthday;
        }
        public Person CopyFromPersonDPO(PersonDPO p)
        {
            RoleViewModel vmRole = new RoleViewModel();
            int roleId = 0;
            foreach (var r in vmRole.ListRole)
            {
                if (r.NameRole == p.RoleName)
                {
                    roleId = r.Id;
                    break;
                }
            }
            if (roleId != 0)
            {
                this.Id = p.Id;
                this.RoleId = roleId;
                this.FirstName = p.FirstName;
                this.LastName = p.LastName;
                this.Birthday = p.GetStringTime(p.Birthday);
            }
            return this;
        }
        public DateTime GetDateTime(string dateTime)
        {
            return DateTime.ParseExact(dateTime, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }
        public string GetStringBirthday(DateTime dateTime)
        {
            return dateTime.ToString("dd.MM.yyyy");
        }
    }
}
