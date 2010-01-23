using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleGrabit
{
    public class Scheduler
    {
        public Scheduler()
        {
            _websites = new List<IWebsiteAutomater>();
        }

        private List<WebconfigsConfig> _websites;

        public delegate void ScheduleAutomation(Type type);
        public event ScheduleAutomation Schedule;

        protected void Monitor()
        {
            foreach (var websiteAutomater in _websites)
            {

            }

            Thread.Sleep(5 * 60 * 1000); //5 minutes
        }

    }
}
