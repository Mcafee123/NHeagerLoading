using System;
using System.Collections.Generic;
using Eg.FluentMappings;
using NHibernate;

namespace Queries
{
    public class ExampleDataCreator
    {
        private readonly ISessionFactory _sessionFactory;

        public ExampleDataCreator(ISessionFactory sessionFactory)
        {
            if (sessionFactory == null)
                throw new ArgumentNullException("sessionFactory");
            _sessionFactory = sessionFactory;
        }

        public void SetUpDatabase()
        {
            using (var session = _sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                ClearDatabase(session);
                CreateMovies(session);
                CreateBook(session);
                tx.Commit();
            }
        }

        private static void ClearDatabase(ISession session)
        {
            session
              .CreateQuery("delete from ActorRole")
              .ExecuteUpdate();

            session
              .CreateQuery("delete from Product")
              .ExecuteUpdate();
        }

        private static void CreateMovies(ISession session)
        {
            session.Save(
              new Movie()
              {
                  Name = "Raiders of the Lost Ark",
                  Description = "Awesome",
                  UnitPrice = 9.59M,
                  Director = "Steven Spielberg",
                  Actors = new List<ActorRole>()
                     {
                       new ActorRole()
                         {
                           Actor = "Harrison Ford",
                           Role = "Indiana Jones"
                         }
                     }
              });

            session.Save(
              new Movie()
              {
                  Name = "The Bucket List",
                  Description = "Good",
                  UnitPrice = 15M,
                  Director = "Rob Reiner",
                  Actors = new List<ActorRole>()
                     {
                       new ActorRole()
                         {
                           Actor = "Jack Nicholson",
                           Role = "Edward Cole"
                         },
                       new ActorRole()
                         {
                           Actor = "Morgan Freeman",
                           Role = "Carter Chambers"
                         }
                     }
              });
        }

        private static void CreateBook(ISession session)
        {
            session.Save(
              new Book()
              {
                  Name = "NHibernate 3.0 Cookbook",
                  Description = "NHibernate examples",
                  UnitPrice = 50M,
                  Author = "Jason Dentler",
                  ISBN = "978-1-849513-04-3"
              });
        }

    }
}