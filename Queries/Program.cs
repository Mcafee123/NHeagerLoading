using System;
using System.Collections.Generic;
using System.Linq;
using Eg.FluentMappings;
using NHibernate;
using NHibernate.Cfg;

namespace Queries
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var nhConfig = new ConfigurationBuilder().Configure();
            var sessionFactory = nhConfig.BuildSessionFactory();

            new ExampleDataCreator(sessionFactory)
              .SetUpDatabase();

            using (var session = sessionFactory.OpenSession())
            using (var tx = session.BeginTransaction())
            {
                RunQueries(session);
                tx.Commit();
            }

            Console.WriteLine("Press any key");
            Console.ReadKey();
        }

        static void RunQueries(ISession session)
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

        static void Show(string heading,
                     IEnumerable<Movie> movies)
        {
            Console.WriteLine(heading);
            foreach (var m in movies)
                ShowMovie(m);
            Console.WriteLine();
        }

        static void Show(string heading, Book book)
        {
            Console.WriteLine(heading);
            ShowBook(book);
            Console.WriteLine();
        }

        static void Show(string heading,
                         IEnumerable<Product> products)
        {
            Console.WriteLine(heading);
            foreach (var p in products)
            {
                if (p is Movie)
                {
                    ShowMovie((Movie)p);
                }
                else if (p is Book)
                {
                    ShowBook((Book)p);
                }
                else
                    ShowProduct(p);
            }
            Console.WriteLine();
        }

        static void Show(string heading,
                         decimal moneyValue)
        {
            Console.WriteLine(heading);
            Console.WriteLine("{0:c}", moneyValue);
            Console.WriteLine();
        }

        static void Show(string heading,
                         IEnumerable<NameAndPrice> results)
        {
            Console.WriteLine(heading);
            foreach (var item in results)
                ShowNameAndPrice(item);
            Console.WriteLine();
        }

        static void ShowNameAndPrice(NameAndPrice item)
        {
            Console.WriteLine("{0:c} {1}",
                              item.Price, item.Name);
        }

        static void ShowProduct(Product p)
        {
            Console.WriteLine("{0:c} {1}",
                              p.UnitPrice, p.Name);
        }

        static void ShowBook(Book b)
        {
            Console.WriteLine("{0:c} {1} (ISBN {2})",
              b.UnitPrice, b.Name, b.ISBN);
        }

        static void ShowMovie(Movie movie)
        {
            var star = movie.Actors
              .Select(actorRole => actorRole.Actor)
              .FirstOrDefault();

            Console.WriteLine("{0:c} {1} starring {2}",
              movie.UnitPrice, movie.Name, star ?? "nobody");
        }
    }
}
