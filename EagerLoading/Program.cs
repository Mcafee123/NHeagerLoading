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
            var mappings = AutoMap.AssemblyOf<NHDossier>(mappingConfig);
            mappings.UseOverridesFromAssemblyOf<NHDossier>();

            var nhConfig = Fluently.Configure()
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                                       connstr => connstr.FromConnectionStringWithKey("db"))
                                                               .AdoNetBatchSize(100))
                                   .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                   .Mappings(m => m.AutoMappings.Add(mappings))
                                   .BuildConfiguration();

            var sessionFactory = nhConfig.BuildSessionFactory();
            Console.WriteLine("NHibernate configured fluently");

            var schemaExport = new SchemaExport(nhConfig);
            schemaExport.Execute(true, true, false);
            Console.WriteLine("DB created");

            var nhDossier = new NHDossier();

            var Erstbewilligung = new NHBewilligung
            {
                Start = new DateTime(2012, 2, 1),
                Ende = new DateTime(2013, 1, 31),
                Bemerkung = "Erstbewilligung",
                Dossier = nhDossier
            };

            var MediWechsel = new NHBewilligung
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

            LoadWithFuture(sessionFactory, nhDossier);

            Console.WriteLine("TestQuery has run");

            Console.ReadKey();
        }

        private static void LoadWithJoinAlias(ISessionFactory sessionFactory, NHDossier nhDossier)
        {
            IList<NHDossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // aliases
                    NHDossier dossierAlias = null;
                    NHBewilligung bewilligungAlias = null;

                    // get dossier with person
                    var query = session.QueryOver<NHDossier>(() => dossierAlias)
                                       .JoinAlias(() => dossierAlias.Bewilligungen, () => bewilligungAlias, JoinType.InnerJoin)
                                       .Where(d => dossierAlias.Id == nhDossier.Id);

                    tx.Commit();
                    list = query.List<NHDossier>();
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

        private static void LoadWithFetch(ISessionFactory sessionFactory, NHDossier nhDossier)
        {
            IList<NHDossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // get dossier with person
                    var query = session.QueryOver<NHDossier>()
                                       .Where(d => d.Id == nhDossier.Id)
                                       .Fetch(d => d.Bewilligungen).Eager
                                       .TransformUsing(Transformers.DistinctRootEntity);

                    tx.Commit();
                    list = query.List<NHDossier>();
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

        private static void LoadWithFuture(ISessionFactory sessionFactory, NHDossier nhDossier)
        {
            IList<NHDossier> list;
            using (var session = sessionFactory.OpenSession())
            {
                using (var tx = session.BeginTransaction())
                {
                    // compose query
                    // get dossier with person
                    var dossier = session.QueryOver<NHDossier>()
                                       .Where(d => d.Id == nhDossier.Id);

                    var bew = session.QueryOver<NHBewilligung>()
                                        .Where(b => b.Dossier.Id == nhDossier.Id)
                                        .Future<NHBewilligung>();

                    tx.Commit();
                    list = dossier.Future<NHDossier>().ToList();
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
