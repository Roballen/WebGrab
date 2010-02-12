using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using ConsoleGrabit.Interfaces;

namespace ConsoleGrabit
{
    public static class WebAutomaterFactory
    
    {
//        TextWriter writer = new StreamWriter("po2.xml");
//serializer.Serialize(writer, po);
//writer.Close();

        private static Webconfigs _webconfigs = null;
        
        public static void GetConfigs(string location)
        {
            var serializer = new XmlSerializer(typeof(Webconfigs));

            var text = new StreamReader(location);
            _webconfigs = (Webconfigs) serializer.Deserialize(text);
        }

        public static WebconfigsConfig GetIndividualConfig(string type)
        {
            return (from config in _webconfigs.Config where config.type == type select config).Single();
        }

        public static List<IWebsiteAutomater> GetWebSiteList()
        {
            var websites = new List<IWebsiteAutomater>();

            var test = Assembly.GetExecutingAssembly().GetTypes();
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes() )
                {
                    if (type == null) continue;

                    //                    if (type != typeof (BaseAutomater)) continue;
                    //                    if (type.Name == "BaseAutomater") continue; // we want to register all subclasses, just not the base class

                    if (type.BaseType == null || type.BaseType.Name == null || type.BaseType.BaseType == null || type.BaseType.BaseType.Name == null || type.BaseType.BaseType.Name != "BaseAutomater")
                        continue;

                    var args = new object[1];
                    args[0] = GetIndividualConfig(type.Name);
                    var o = Activator.CreateInstance(type, args);
                    websites.Add((IWebsiteAutomater)o);
                }
            

            return websites;
        }

    }
}
