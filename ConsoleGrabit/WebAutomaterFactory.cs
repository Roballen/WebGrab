using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit
{
    public static class WebAutomaterFactory
    {
        
        public static List<IWebsiteAutomater> GetWebSiteList()
        {
            var websites = new List<IWebsiteAutomater>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName != null)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type != null)
                        {
                            if (type == typeof(BaseAutomater))
                            {
                                var args = new object[1];
                                var o = Activator.CreateInstance(type);
                                websites.Add( (IWebsiteAutomater)o );
                            }
                        }
                    }
                }
            }

//                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
//            {
//
//                if ( assembly.FullName != null && assembly.FullName.ToLower().Contains("floodbusinesslogic"))
//                {
//                    foreach (var type in assembly.GetTypes())
//                    {
//
//
//                        if ( type != null && type.Namespace != null )//&& type.BaseType != null && type.BaseType.Name != null)
//                        {
//                            if (type.Namespace.ToLower() == "floodbusinesslogic.legacyservices")
        }

    }
}
