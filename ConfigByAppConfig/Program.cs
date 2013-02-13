using System;
using NHibernate.Tool.hbm2ddl;

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
            Console.WriteLine("NHibernate configured!");

            var schemaExport = new SchemaExport(nhConfig);
            schemaExport.Create(false, true);
            Console.WriteLine("DB Schema created!");

            Console.ReadKey();
        }
    }
}
