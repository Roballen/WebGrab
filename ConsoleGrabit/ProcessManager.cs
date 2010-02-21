using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;
using ConsoleGrabit.Repository;

namespace ConsoleGrabit
{
    public class ProcessManager
    {

        public static bool NeedtoCheck(WebconfigsConfig _config, IRepository repository)
        {
            var interval = _config.intervalSpecified ? _config.interval : 8 / _config.checksperday; // will check equally throughout the day

            if (DateTime.Now > _config.starttime) // if we are past the start time then start check
            {
                if (_pullstoday == 0 || _lastupdate == null) // if we haven't done it today go for it
                    return true;
                if (_pullstoday < _config.checksperday && (DateTime.Now > ((DateTime)_lastupdate).AddHours(interval))) // if we have done it today, check how many time and the intervals we are to check
                    return true;
            }
            return false;
        }

        public static CountyPull Process(IWebsiteAutomater automater)
        {
                var _pull = new CountyPull {County = automater.County()};
            try
                {
                    automater.NavigateToLeadList();
                    _pull.Leads = automater.ProcessMultiple();
                }
                catch (Exception e)
                {
                    _pull.Messages.Add(new Message() { Content = e.Message, Messagetype = MessageType.Error });
                }

            return _pull;

        }


    }
}
