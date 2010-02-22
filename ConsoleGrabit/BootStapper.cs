using System;
using ConsoleGrabit.Repository;
using Db4objects.Db4o;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace ConsoleGrabit
{
    public class ConsoleGrabItRegistry : Registry
    {
        public ConsoleGrabItRegistry()
        {

//            Scan(Registry =>
//                     {
//                         Registry.AssemblyContainingType<IRepository>();
//                         Registry.AssemblyContainingType<Db4oServer>();
//
//                     });

            ForSingletonOf<Db4oServer>().Use<Db4oServer>()
                .Ctor<String>("pathToDbFile").Is("c:\\db40\\County.yap") //  .EqualToAppSetting("Db40Location")
                .Ctor<int>("port").Is(0)
                .Ctor<bool>().Is(true)
                ;


            For<IObjectContainer>().Use(context => context.GetInstance<Db4oServer>().GetClient());
            For<IObjectServer>().Use(context => context.GetInstance<Db4oServer>().Server);
            For<IRepository>().Use(context => context.GetInstance<Db4oRepository>());
        }
    }

    public static class ConfigureStructureMap
    {
        public static void BootStrapIt()
        {
            ObjectFactory.Initialize( x=> x.AddRegistry( new ConsoleGrabItRegistry() ));
        }
    }
}
