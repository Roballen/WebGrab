using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;

namespace ConsoleGrabit.Interfaces
{
    public interface IWebsiteAutomater
    {
        //CountyPull Automate();
//        Lead GetLeadFromDetailView();
//        Lead GetLeadFromDocumentView();
        void NavigateToLeadList();
        IList<Lead> ProcessMultiple();
        bool AreRecordsToProcess();
        string County();
//        Lead ProcessDetail(object detail);
//        Lead ProcessDocument(Lead lead, object detail);
        //Lead ProcessSingle();
//        void ProcessDetailView();
//        void ProcessDocument();
    }
}
