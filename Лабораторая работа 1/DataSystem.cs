using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Лабораторая_работа_1
{
    public class DataSystem
    {
        private int _hour;
        private int _minutes;
        private int _seconds;
        private int _secondsToAdd;

        public int _Hour { get; set; }
        public int _Minutes { get; set; }
        public int _Seconds { get; set; }

        private int _SecondsInHour = 3600;
        private int _SecondsInMinute = 60;

        private int _MinutesInHour = 60;
        public int _SecondsToAdd { get; set; }

        public DataSystem(int _hour, int _minutes, int _seconds, int _secondsToAdd)
        {
            _Hour = _hour;
            _Minutes = _minutes;
            _Seconds = _seconds;
            _SecondsToAdd = _secondsToAdd;
        }

        public int ReturnSeconds(int hour, int minutes, int seconds)
        {
            return _SecondsInHour * hour + _SecondsInMinute * minutes + seconds;
        }
        
        public (int, int, int) AddSomeSeconds(int hour, int minutes, int seconds, int secondstoadd)
        {
            int newHours = hour; int newMinutes = minutes; int newSeconds = seconds + secondstoadd;
            if (newSeconds >= _SecondsInMinute)
            {
                while (newSeconds >= _SecondsInMinute)
                {
                    newMinutes++;
                    newSeconds -= _SecondsInMinute;
                }
            }
            if (newMinutes >= _MinutesInHour)
            {
                while (newMinutes >= _MinutesInHour)
                {
                    newHours++;
                    newMinutes -= _MinutesInHour;
                }
            }

            return (newHours, newMinutes, newSeconds);
        }
    }
}
