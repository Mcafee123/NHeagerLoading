using System;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Dialect;

namespace ConfigByCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var nhConfig = new Configuration()
                .Proxy(proxy => proxy.ProxyFactoryFactory<DefaultProxyFactoryFactory>())
                .DataBaseIntegration(db =>
                    {
                        db.Dialect<MsSql2008Dialect>();
                        db.ConnectionStringName = "db";
                        db.BatchSize = 100;
                    })
                .AddAssembly("Eg.Core");

            var sessionFactory = nhConfig.BuildSessionFactory();
            Console.WriteLine("NHibernate Configured!");
            Console.ReadKey();
        }
    }
}
