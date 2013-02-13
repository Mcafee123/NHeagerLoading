using System;
using System.Collections.Generic;
using Eg.FluentMappings;
using NHibernate;

namespace Queries
{
    public class Queries
    {
        private readonly ISession _session;

        public Queries(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");
            _session = session;
        }

        public IEnumerable<Movie> GetMoviesDirectedBy(string directorName)
        {
            return _session.QueryOver<Movie>()
              .Where(m => m.Director == directorName)
              .List();
        }

        public IEnumerable<Movie> GetMoviesWith(string actorName)
        {
            return _session.QueryOver<Movie>()
              .OrderBy(m => m.UnitPrice).Asc
              .Inner.JoinQueryOver<ActorRole>(m => m.Actors)
              .Where(a => a.Actor == actorName)
              .List();
        }

        public Book GetBookByISBN(string isbn)
        {
            return _session.QueryOver<Book>()
              .Where(b => b.ISBN == isbn)
              .SingleOrDefault();
        }

        public IEnumerable<Product> GetProductByPrice(decimal minPrice, decimal maxPrice)
        {
            return _session.QueryOver<Product>()
              .Where(p => p.UnitPrice >= minPrice
                          && p.UnitPrice <= maxPrice)
              .OrderBy(p => p.UnitPrice).Asc
              .List();
        }
    }
}
