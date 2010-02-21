using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Models;
using Db4objects.Db4o;

namespace ConsoleGrabit.Repository
{
    public class Db4oRepository
    {
        public CountyPull GetSingle()
        {
            try
            {
                IObjectContainer db = Db4oFactory.OpenFile()
                IObjectContainer db = Db4oEmbedded.OpenFile(Db4oEmbedded.NewConfiguration(), Properties.Settings.Default.Db40Location);
                db.
            }
            finally
            {
                
            }
        }
    }
}
