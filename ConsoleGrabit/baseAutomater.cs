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
        
        protected DateTime? _lastupdate;
        protected int _pullstoday;
        protected WebconfigsConfig _config;

        protected BaseAutomater(WebconfigsConfig config)
        {
            _config = config;
            _lastupdate = null;
            _pullstoday = 0;
        }

        protected bool NeedtoCheck()
        {
            var interval = _config.intervalSpecified ? _config.interval : 8 / _config.checksperday; // will check equally throughout the day

            if ( DateTime.Now > _config.starttime ) // if we are past the start time then start check
            {
                if (_pullstoday == 0 || _lastupdate == null) // if we haven't done it today go for it
                    return true;
                if ( _pullstoday < _config.checksperday && (DateTime.Now > ((DateTime)_lastupdate).AddHours(interval))) // if we have done it today, check how many time and the intervals we are to check
                    return true;
            }
            return false;
        }

    }
}
