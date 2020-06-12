using System;
using Microsoft.ML.Data;

namespace query_sql
{
    partial class Program
    {
        public class TaxiTrip
        {
            private DateTime _time;

            public TaxiTrip(DateTime time, float count)
            {
                _time = time;
                FareAmount = count;
            }
            public DateTime Time => _time;
            [ColumnName("Label")]
            public float FareAmount { get; }
            //public float Is0000 => _time.ToString("HH:mm") == "00:00"? 1:0;
            public float Is0100 => _time.ToString("HH:mm") == "01:00" ? 1 : 0;
            public float Is0200 => _time.ToString("HH:mm") == "02:00" ? 1 : 0;
            public float Is0300 => _time.ToString("HH:mm") == "03:00" ? 1 : 0;
            public float Is0400 => _time.ToString("HH:mm") == "04:00" ? 1 : 0;
            public float Is0500 => _time.ToString("HH:mm") == "05:00" ? 1 : 0;
            public float Is0600 => _time.ToString("HH:mm") == "06:00" ? 1 : 0;
            public float Is0700 => _time.ToString("HH:mm") == "07:00" ? 1 : 0;
            public float Is0800 => _time.ToString("HH:mm") == "08:00" ? 1 : 0;
            public float Is0900 => _time.ToString("HH:mm") == "09:00" ? 1 : 0;
            public float Is1000 => _time.ToString("HH:mm") == "10:00" ? 1 : 0;
            public float Is1100 => _time.ToString("HH:mm") == "11:00" ? 1 : 0;
            public float Is1200 => _time.ToString("HH:mm") == "12:00" ? 1 : 0;
            public float Is1300 => _time.ToString("HH:mm") == "13:00" ? 1 : 0;
            public float Is1400 => _time.ToString("HH:mm") == "14:00" ? 1 : 0;
            public float Is1500 => _time.ToString("HH:mm") == "15:00" ? 1 : 0;
            public float Is1600 => _time.ToString("HH:mm") == "16:00" ? 1 : 0;
            public float Is1700 => _time.ToString("HH:mm") == "17:00" ? 1 : 0;
            public float Is1800 => _time.ToString("HH:mm") == "18:00" ? 1 : 0;
            public float Is1900 => _time.ToString("HH:mm") == "19:00" ? 1 : 0;
            public float Is2000 => _time.ToString("HH:mm") == "20:00" ? 1 : 0;
            public float Is2100 => _time.ToString("HH:mm") == "21:00" ? 1 : 0;
            public float Is2200 => _time.ToString("HH:mm") == "22:00" ? 1 : 0;
            public float Is2300 => _time.ToString("HH:mm") == "23:00" ? 1 : 0;
            //public float Is1 => _time.Day == 1 ? 1 : 0;
            public float Is2 => _time.Day == 2 ? 1 : 0;
            public float Is3 => _time.Day == 3 ? 1 : 0;
            public float Is4 => _time.Day == 4 ? 1 : 0;
            public float Is5 => _time.Day == 5 ? 1 : 0;
            public float Is6 => _time.Day == 6 ? 1 : 0;
            public float Is7 => _time.Day == 7 ? 1 : 0;
            public float Is8 => _time.Day == 8 ? 1 : 0;
            public float Is9 => _time.Day == 9 ? 1 : 0;
            public float Is10 => _time.Day == 10 ? 1 : 0;
            public float Is11 => _time.Day == 11 ? 1 : 0;
            public float Is12 => _time.Day == 12 ? 1 : 0;
            public float Is13 => _time.Day == 13 ? 1 : 0;
            public float Is14 => _time.Day == 14 ? 1 : 0;
            public float Is15 => _time.Day == 15 ? 1 : 0;
            public float Is16 => _time.Day == 16 ? 1 : 0;
            public float Is17 => _time.Day == 17 ? 1 : 0;
            public float Is18 => _time.Day == 18 ? 1 : 0;
            public float Is19 => _time.Day == 19 ? 1 : 0;
            public float Is20 => _time.Day == 20 ? 1 : 0;
            public float Is21 => _time.Day == 21 ? 1 : 0;
            public float Is22 => _time.Day == 22 ? 1 : 0;
            public float Is23 => _time.Day == 23 ? 1 : 0;
            public float Is24 => _time.Day == 24 ? 1 : 0;
            public float Is25 => _time.Day == 25 ? 1 : 0;
            public float Is26 => _time.Day == 26 ? 1 : 0;
            public float Is27 => _time.Day == 27 ? 1 : 0;
            public float Is28 => _time.Day == 28 ? 1 : 0;
            public float Is29 => _time.Day == 29 ? 1 : 0;
            public float Is30 => _time.Day == 30 ? 1 : 0;
            public float Is31 => _time.Day == 31 ? 1 : 0;


        }
    }
}
