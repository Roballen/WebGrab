using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Interfaces;
using ConsoleGrabit.Models;

namespace ConsoleGrabit
{
    public class ProcessManager
    {

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
