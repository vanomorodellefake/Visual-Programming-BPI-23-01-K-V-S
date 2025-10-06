using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторная_работа_2
{
    public class CheckerMoreThanZero : IDataErrorInfo
    {
        public int N { get; set; }
        public int K { get; set; }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "R5TextN":
                        if (!CheckMoreThanZero(N))
                        {
                            error = "Значение должно быть больше 0";
                        }
                        break;
                    case "R5TextK":
                        if (!CheckMoreThanZero(K))
                        {
                            error = "Значение должно быть больше 0";
                        }
                        break;
                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        private bool CheckMoreThanZero(int ToCheck)
        {
            if (ToCheck > 0) return true;
            return false;
        }
    }
}
