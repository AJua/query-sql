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
            public float Is1 => _time.Day == 1 ? 1 : 0;
            public float Is2 => _time.Day == 2 ? 1 : 0;
            public float Is3 => _time.Day == 3 ? 1 : 0;
            public float Is4 => _time.Day == 4 ? 1 : 0;
            public float Is5 => _time.Day == 5 ? 1 : 0;
            public float Is6 => _time.Day == 6 ? 1 : 0;
            public float Is7 => _time.Day == 7 ? 1 : 0;
            public float Is8 => _time.Day == 8 ? 1 : 0;
            public float Is9 => _time.Day == 9 ? 1 : 0;
            public float Is10 => _time.Day == 10 ? 1 : 0;

        }
    }
}
