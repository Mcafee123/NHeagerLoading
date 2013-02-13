using System;
using System.Collections.Generic;
using System.Linq;
using Eg.Core.Data;
using Eg.FluentMappings;
using NHibernate;
using NHibernate.Cfg;
using log4net.Config;

namespace Queries
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Configuration nhConfig = new ConfigurationBuilder().Configure();
            ISessionFactory sessionFactory = nhConfig.BuildSessionFactory();

            new ExampleDataCreator(sessionFactory)
                .SetUpDatabase();

            // THE DIRECT WAY
            // --------------
            //using (var session = sessionFactory.OpenSession())
            //using (var tx = session.BeginTransaction())
            //{
            //    RunQueries(session);
            //    tx.Commit();
            //}
            // THE DAO WAY
            // -----------
            RunDao(sessionFactory);

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        private static void RunDao(ISessionFactory sessionFactory)
        {
            var mao = new MovieAccessObject(sessionFactory);

            Show("Movies directed by Spielberg:",
                mao.GetMoviesDirectedBy("Steven Spielberg"));

            Show("Movies with Morgan Freeman:",
                 mao.GetMoviesWith(
                     "Morgan Freeman"));

            Show("This book:",
                 mao.GetBookByISBN(
                     "978-1-849513-04-3"));

            Show("Cheap products:",
                 mao.GetProductByPrice(0M, 15M));
        }

        private static void RunQueries(ISession session)
        {
            var queries = new Queries(session);

            Show("Movies directed by Spielberg:",
                 queries.GetMoviesDirectedBy(
                     "Steven Spielberg"));

            Show("Movies with Morgan Freeman:",
                 queries.GetMoviesWith(
                     "Morgan Freeman"));

            Show("This book:",
                 queries.GetBookByISBN(
                     "978-1-849513-04-3"));

            Show("Cheap products:",
                 queries.GetProductByPrice(0M, 15M));
        }

        private static void Show(string heading,
                                 IEnumerable<Movie> movies)
        {
            Console.WriteLine(heading);
            foreach (Movie m in movies)
                ShowMovie(m);
            Console.WriteLine();
        }

        private static void Show(string heading, Book book)
        {
            Console.WriteLine(heading);
            ShowBook(book);
            Console.WriteLine();
        }

        private static void Show(string heading,
                                 IEnumerable<Product> products)
        {
            Console.WriteLine(heading);
            foreach (Product p in products)
            {
                if (p is Movie)
                {
                    ShowMovie((Movie) p);
                }
                else if (p is Book)
                {
                    ShowBook((Book) p);
                }
                else
                    ShowProduct(p);
            }
            Console.WriteLine();
        }

        private static void Show(string heading,
                                 decimal moneyValue)
        {
            Console.WriteLine(heading);
            Console.WriteLine("{0:c}", moneyValue);
            Console.WriteLine();
        }

        private static void Show(string heading,
                                 IEnumerable<NameAndPrice> results)
        {
            Console.WriteLine(heading);
            foreach (NameAndPrice item in results)
                ShowNameAndPrice(item);
            Console.WriteLine();
        }

        private static void ShowNameAndPrice(NameAndPrice item)
        {
            Console.WriteLine("{0:c} {1}",
                              item.Price, item.Name);
        }

        private static void ShowProduct(Product p)
        {
            Console.WriteLine("{0:c} {1}",
                              p.UnitPrice, p.Name);
        }

        private static void ShowBook(Book b)
        {
            Console.WriteLine("{0:c} {1} (ISBN {2})",
                              b.UnitPrice, b.Name, b.ISBN);
        }

        private static void ShowMovie(Movie movie)
        {
            string star = movie.Actors
                               .Select(actorRole => actorRole.Actor)
                               .FirstOrDefault();

            Console.WriteLine("{0:c} {1} starring {2}",
                              movie.UnitPrice, movie.Name, star ?? "nobody");
        }
    }
}