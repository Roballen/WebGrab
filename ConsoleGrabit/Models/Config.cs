using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleGrabit.Models
{
    public class Coordinate
    {
        public int X { get; set;}
        public int Y { get; set;}
    }

    public class Config
    {

        public Config()
        {
            _performancetweaks = new Dictionary<string, int>();
            _positionals = new Dictionary<string, Coordinate>();
            _username = "";
            _starttime = DateTime.Now;
            _priority = 1;
            _password = "";
            _interval = 4;
            _daysback = 1;
            _county = "";
            _checkperday = 1;
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string County
        {
            get { return _county; }
            set { _county = value; }
        }

        public DateTime Starttime
        {
            get { return _starttime; }
            set { _starttime = value; }
        }

        public int Checkperday
        {
            get { return _checkperday; }
            set { _checkperday = value; }
        }

        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }

        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public int Daysback
        {
            get { return _daysback; }
            set { _daysback = value; }
        }

        public Dictionary<string, Coordinate> Positionals
        {
            get { return _positionals; }
            set { _positionals = value; }
        }


        public Dictionary<string, int> Performancetweaks
        {
            get { return _performancetweaks; }
            set { _performancetweaks = value; }
        }

        private Dictionary<string, int> _performancetweaks;
        private Dictionary<string, Coordinate> _positionals;
        private string _password;
        private string _username;
        private string _county;
        private DateTime _starttime;
        private int _checkperday;
        private int _interval;
        private int _priority;
        private int _daysback;
    }
}
