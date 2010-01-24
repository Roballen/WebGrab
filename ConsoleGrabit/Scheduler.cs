using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit
{
    public class Scheduler
    {
        public Scheduler( List<IWebsiteAutomater> websites )
        {
            _websites = websites;
        }

        private List<IWebsiteAutomater> _websites;

        public delegate void ScheduleAutomation(string messages);
        public event ScheduleAutomation Schedule;

        protected void Monitor()
        {
            foreach (IWebsiteAutomater websiteAutomater in _websites)
            {
                if (  )
            }

            Thread.Sleep(5 * 60 * 1000); //5 minutes
        }





    }
}
