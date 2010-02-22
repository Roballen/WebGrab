using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;

namespace ConsoleGrabit.Repository
{
    public interface IRepository
    {
        int GetCompletedCount(string countyname, DateTime date);
        IList<CountyPull> GetMultipleByDate(DateTime date, string county);
    }
}
