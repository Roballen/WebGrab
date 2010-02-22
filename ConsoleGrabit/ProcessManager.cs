using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;
using ConsoleGrabit.Repository;
using StructureMap;

namespace ConsoleGrabit
{
    public class ProcessManager
    {

        public static bool NeedtoCheck(Config config, IRepository repository)
        {

            if (config.Checkperday < 1)
                return false;

            DateTime? lastupdate = DateTime.MinValue;
            int pullstoday = 0;

            // get count of completed orders
            pullstoday  = repository.GetCompletedCount(config.County, DateTime.Now);
            var updatestoday = repository.GetMultipleByDate(DateTime.Now, config.County);

            foreach (var countyPull in updatestoday)
            {
                if (countyPull.Time > lastupdate)
                    lastupdate = countyPull.Time;
            }


            var interval = config.Interval > 0 ? config.Interval : 8 / config.Checkperday; // will check equally throughout the day

            if (DateTime.Now.TimeOfDay > config.Starttime.TimeOfDay) // if we are past the start time then start check
            {
                if (pullstoday == 0 || lastupdate == null) // if we haven't done it today go for it
                    return true;
                if (pullstoday < config.Checkperday && (DateTime.Now > ((DateTime)lastupdate).AddHours(interval))) // if we have done it today, check how many time and the intervals we are to check
                    return true;
            }
            return false;
        }

        public static CountyPull Process(IWebsiteAutomater automater)
        {

            var pull = new CountyPull {County = automater.Config().County};

            if (! NeedtoCheck( automater.Config(), ObjectFactory.GetInstance<IRepository>() ))
            {
                pull.Status = CountyPullStatus.DailyMaximumReached;
                return pull;
            }

            try
                {
                    automater.NavigateToLeadList();
                    pull.Leads = automater.ProcessMultiple();
                }
                catch (Exception e)
                {
                    pull.Status = CountyPullStatus.Error;
                    pull.Messages.Add(new Message() { Content = e.Message, Messagetype = MessageType.Error });
                }

            return pull;

        }


    }
}
