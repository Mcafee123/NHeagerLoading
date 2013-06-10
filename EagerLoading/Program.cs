using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

namespace EagerLoading
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            App_Start.NHibernateProfilerBootstrapper.PreStart();

            var mappingConfig = new MappingCfg();
            var autoMappings = AutoMap.AssemblyOf<Dossier>(mappingConfig);
            //autoMappings.UseOverridesFromAssemblyOf<NHDossier>();

            // debug out mappings
            var path = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\GeneratedMappings"));

            var nhConfig = Fluently.Configure()
                                   .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                                       connstr => connstr.FromConnectionStringWithKey("db"))
                                                               .AdoNetBatchSize(100)
                                                               .IsolationLevel(IsolationLevel.ReadCommitted))
                                   .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                   .Mappings(m => m.AutoMappings.Add(autoMappings).ExportTo(path.FullName))
                //.Mappings(m => m.FluentMappings.AddFromAssemblyOf<EagerLoading.Mappings.DossierMap>().ExportTo(path.FullName))
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
            using (var tx = session.BeginTransaction())
            {
                session.Save(nhDossier);
                session.Save(Erstbewilligung);
                session.Save(MediWechsel);

                session.Save(Erstbewilligung);
                tx.Commit();
            }
            Console.WriteLine("Testdata inserted");

            NHibernateProfiler.Initialize();

            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            //LoadWithJoinAlias(sessionFactory, nhDossier);
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            //LoadWithFetch(sessionFactory, nhDossier);
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine(".");
            LoadWithFuture(sessionFactory, nhDossier);
            Console.WriteLine(".");
            Console.WriteLine(".");
            Console.WriteLine("TestQueries executed");

            Console.ReadKey();
        }

        private static void LoadWithJoinAlias(ISessionFactory sessionFactory, Dossier nhDossier)
        {
            IList<Dossier> list;
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                // compose query
                // aliases
                Dossier dossierAlias = null;
                IList<Bewilligung> bewilligungAlias = null;

                // get dossier with person
                list = session.QueryOver(() => dossierAlias)
                              .JoinAlias(() => dossierAlias.Bewilligungen, () => bewilligungAlias,
                                         JoinType.LeftOuterJoin)
                              .Where(d => dossierAlias.Id == nhDossier.Id)
                              .TransformUsing(new DistinctRootEntityResultTransformer())
                              .List<Dossier>();

                tx.Commit();
            }

            try
            {
                foreach (var d in list)
                {
                    Console.WriteLine(d.Id);
                    foreach (var b in d.Bewilligungen)
                        Console.WriteLine(b.Bemerkung);
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
            using (var tx = session.BeginTransaction())
            {
                // compose query
                // get dossier with person
                var query = session.QueryOver<Dossier>()
                                   .Fetch(d => d.Bewilligungen).Eager
                                   .Where(d => d.Id == nhDossier.Id)
                                   .TransformUsing(Transformers.DistinctRootEntity);

                tx.Commit();
                list = query.List<Dossier>();
            }

            try
            {
                foreach (var d in list)
                {
                    Console.WriteLine(d.Id);
                    foreach (var b in d.Bewilligungen)
                        Console.WriteLine(b.Bemerkung);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.GetBaseException().Message);
            }
        }

        private static void LoadWithFuture(ISessionFactory sessionFactory, Dossier nhDossier)
        {
            Dossier doss;
            IEnumerable<Bewilligung> dossBew;
            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                // dossier id
                var dossierId = nhDossier.Id;
                // compose query
                //IList<Bewilligung> bewilligungAlias = null;
                //Dossier dossierAlias = null;

                // get dossier with person
                Dossier dossHeader = null;
                var dossierList = session.QueryOver<Dossier>()
                                         .SelectList(builder => builder
                                             .Select(d => d.Id).WithAlias(() => dossHeader.Id)
                                             )
                                         .Where(d => d.Id == dossierId)
                                         .TransformUsing(Transformers.AliasToBean<Dossier>())
                                         .Future<Dossier>();

                dossBew = session.QueryOver<Bewilligung>()
                                 .Where(d => d.Dossier.Id == dossierId)
                                 .Future<Bewilligung>();


                //bew.GetEnumerator().MoveNext();
                doss = dossierList.FirstOrDefault();
                //doss.Bewilligungen.GetEnumerator().MoveNext();
                tx.Commit();
            }
            if (doss == null)
                return;
            foreach (var bew in dossBew)
            {
                Console.WriteLine(bew.ToString());
                doss.Bewilligungen.Add(bew);
            }

            try
            {
                //foreach (var d in list)
                //{
                //    Console.WriteLine(d.Id);
                //    foreach (var b in d.Bewilligungen)
                //    {
                //        Console.WriteLine(b.Bemerkung);
                //    }
                //}

                Console.WriteLine("second:");
                foreach (var b in doss.Bewilligungen)
                    Console.WriteLine(b.Bemerkung);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: {0}", ex.GetBaseException().Message);
            }
        }
    }
}