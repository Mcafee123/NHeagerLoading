//using EagerLoading.Mappings;
using EagerLoading.NHObj;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.SqlCommand;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading
{
    class Program
    {
        static void Main(string[] args)
        {
            NHibernateProfiler.Initialize();
            var mappingConfig = new MappingCfg();
            var autoMappings = AutoMap.AssemblyOf<Dossier>(mappingConfig);
            //autoMappings.UseOverridesFromAssemblyOf<NHDossier>();

            // debug out mappings
            var path = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\GeneratedMappings"));

            var nhConfig = Fluently.Configure()
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                                       connstr => connstr.FromConnectionStringWithKey("db"))
                                                               .AdoNetBatchSize(100))
                                   .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                   .Mappings(m => m.AutoMappings.Add(autoMappings).ExportTo(path.FullName))
                                   //.Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHDossierMap>().ExportTo(path.FullName))
                                   .BuildConfiguration();

            var sessionFactory = nhConfig.BuildSessionFactory();
            Console.WriteLine("NHibernate configured fluently");

            var schemaExport = new SchemaExport(nhConfig);
            schemaExport.Execute(true, true, false);
            Console.WriteLine("DB created");

            var nhDossier = new Dossier();

            var Erstbewilligung = new Bewilligung
            {
                Start = new DateTime(2012, 2, 1),
                Ende = new DateTime(2013, 1, 31),
                Bemerkung = "Erstbewilligung",
                Dossier = nhDossier
            };

            var MediWechsel = new Bewilligung
            {
                Start = new DateTime(2012, 6, 5),
                Dossier = nhDossier,
                Bemerkung = "Medikamentenwechsel",
                //Grundbewilligung = Erstbewilligung
            };

            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    session.Save(nhDossier);
                    session.Save(Erstbewilligung);
                    session.Save(MediWechsel);

                    session.Save(Erstbewilligung);
                    tx.Commit();
                }
            }
            Console.WriteLine("Testdata inserted");

            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            LoadWithJoinAlias(sessionFactory, nhDossier);
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            LoadWithJoinAlias(sessionFactory, nhDossier);
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine("TestQuery has run");

            Console.ReadKey();
        }

        private static void LoadWithJoinAlias(ISessionFactory sessionFactory, Dossier nhDossier)
        {
            IList<Dossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // aliases
                    Dossier dossierAlias = null;
                    Bewilligung bewilligungAlias = null;

                    // get dossier with person
                    var query = session.QueryOver<Dossier>(() => dossierAlias)
                                       .JoinAlias(() => dossierAlias.Bewilligungen, () => bewilligungAlias, JoinType.InnerJoin)
                                       .Where(d => dossierAlias.Id == nhDossier.Id);

                    tx.Commit();
                    list = query.List<Dossier>();
                 }
            }

            try
            {
                foreach (var d in list)
                {
                    Console.WriteLine(d.Id);
                    foreach (var b in d.Bewilligungen)
                    {
                        Console.WriteLine(b.Bemerkung);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.GetBaseException().Message);
            }
        }

        private static void LoadWithFetch(ISessionFactory sessionFactory, Dossier nhDossier)
        {
            IList<Dossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // get dossier with person
                    var query = session.QueryOver<Dossier>()
                                       .Where(d => d.Id == nhDossier.Id)
                                       .Fetch(d => d.Bewilligungen).Eager
                                       .TransformUsing(Transformers.DistinctRootEntity);

                    tx.Commit();
                    list = query.List<Dossier>();
                }
            }

            try
            {
                foreach (var d in list)
                {
                    Console.WriteLine(d.Id);
                    foreach (var b in d.Bewilligungen)
                    {
                        Console.WriteLine(b.Bemerkung);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.GetBaseException().Message);
            }
        }

        private static void LoadWithFuture(ISessionFactory sessionFactory, Dossier nhDossier)
        {
            IList<Dossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // get dossier with person
                    var dossier = session.QueryOver<Dossier>()
                                       .Where(d => d.Id == nhDossier.Id);

                    var bew = session.QueryOver<Bewilligung>()
                                        .Where(b => b.Dossier.Id == nhDossier.Id)
                                        .Future<Bewilligung>();

                    tx.Commit();
                    list = dossier.Future<Dossier>().ToList();
                }
            }

            try
            {
                foreach (var d in list)
                {
                    Console.WriteLine(d.Id);
                    foreach (var b in d.Bewilligungen)
                    {
                        Console.WriteLine(b.Bemerkung);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.GetBaseException().Message);
            }
        }
    }
}
