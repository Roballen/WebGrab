using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit
{
    public abstract class BaseAutomater
    {
        public DateTime? Lastupdate
        {
            get { return _lastupdate; }
            set { _lastupdate = value; }
        }

        public int Pullstoday
        {
            get { return _pullstoday; }
            set { _pullstoday = value; }
        }

        protected DateTime? _lastupdate;
        protected int _pullstoday;

        public BaseAutomater()
        {
            _lastupdate = null;
            _pullstoday = 0;
        }
    }
}
