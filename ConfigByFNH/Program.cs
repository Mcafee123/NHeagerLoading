using System;
using Eg.FluentMappings.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Bytecode;

namespace ConfigByFNH
{
    class Program
    {
        static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.NHibernate.NHibernateProfiler.Initialize();

            var nhConfig = Fluently.Configure()
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                                       connstr => connstr.FromConnectionStringWithKey("db"))
                                                               .AdoNetBatchSize(100))
                                   .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                   .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<ProductMapping>())
                                   .BuildConfiguration();

            var sessionFactory = nhConfig.BuildSessionFactory();
            Console.WriteLine("NHibernate configured fluently");



            Console.ReadKey();
        }
    }
}
