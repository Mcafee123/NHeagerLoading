using System;
using NHibernate.Cfg;

namespace ConfigByAppConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            //var nhConfig = new Configuration().Configure();
            var nhConfig = new ConfigurationBuilder().Configure();
            var sessionFactory = nhConfig.BuildSessionFactory();
            Console.WriteLine("NHibernate Configured!");
            Console.ReadKey();
        }
    }
}
