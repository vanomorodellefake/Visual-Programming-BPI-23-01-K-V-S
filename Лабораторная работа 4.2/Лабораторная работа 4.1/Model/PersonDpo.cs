using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WpfApp.ViewModel;

namespace WpfApp.Model
{
    public class PersonDpo : INotifyPropertyChanged
    {
        public int Id { get; set; }
        private string roleName { get; set; }
        public string RoleName
        {
            get { return roleName; }
            set
            {
                roleName = value;
                OnPropertyChanged("RoleName");
            }
        }

        private string firstName { get; set; }
        public string FirstName
        {
            get { return firstName; }
            set
            {
                firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private DateTime birthday;
        public DateTime Birthday
        {
            get { return birthday; }
            set
            {
                birthday = value;
                OnPropertyChanged("Birthday");
            }
        }

        public PersonDpo() { }
        public PersonDpo(int id, int roleId, string firstName, string lastName, DateTime birthday)
        {
            this.Id = id; 
            this.RoleName = roleName;
            this.FirstName = firstName; 
            this.LastName = lastName; 
            this.Birthday = birthday;
        }
        public PersonDpo ShallowCopy()
        {
            return (PersonDpo)this.MemberwiseClone();
        }
        public PersonDpo CopyFromPerson(Person person)
        {
            PersonDpo perDpo = new PersonDpo(); RoleViewModel vmRole = new RoleViewModel(); string role = string.Empty;
            foreach (var r in vmRole.ListRole)
            {
                if (r.Id == person.RoleId)
                {
                    role = r.NameRole; break;
                }
            }
            if (role != string.Empty)
            {
                perDpo.Id = person.Id; 
                perDpo.RoleName = role;
                perDpo.FirstName = person.FirstName; 
                perDpo.LastName = person.LastName; 
                perDpo.Birthday = person.Birthday;
            }
            return perDpo;
        }
        public event PropertyChangedEventHandler PropertyChanged; 
        //[NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
