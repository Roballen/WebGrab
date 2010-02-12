using System;
using System.Collections.Generic;

namespace ConsoleGrabit.Models
{
    public class CountyPull
    {
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; }
        }

        public string County
        {
            get { return _county; }
            set { _county = value; }
        }

        public IList<Message> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        public IList<Lead> Leads
        {
            get { return _leads; }
            set { _leads = value; }
        }

        private DateTime _time;
        private string _county = "";
        private IList<Message> _messages;
        private IList<Lead> _leads;

        public CountyPull()
        {
            _time = DateTime.Now;
            _messages = new List<Message>();
            _leads = new List<Lead>();
            _county = "";
        }
    }
}
