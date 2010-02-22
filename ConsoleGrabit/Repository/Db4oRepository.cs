using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;

namespace ConsoleGrabit.Repository
{
    public class Db4oRepository : IRepository
    {

        private IObjectContainer _db;

        public Db4oRepository(IObjectContainer container)
        {
            _db = container;
        }

        public int GetCompletedCount(string countyname, DateTime date)
        {
            return (from CountyPull o in _db
                   where o.Time.Date == date.Date && o.Status == CountyPullStatus.Complete
                   select o).Count();
        }

        public IList<CountyPull> GetMultipleByDate(DateTime date, string county)
        {
            return (from CountyPull o in _db where o.Time.Date == date.Date && o.County == county orderby o.Time descending  select o ).ToList();
        }
    }
}
