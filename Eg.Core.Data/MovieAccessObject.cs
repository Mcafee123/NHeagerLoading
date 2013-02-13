using System.Collections.Generic;
using Eg.FluentMappings;
using NHibernate;

namespace Eg.Core.Data
{
    public class MovieAccessObject: DataAccessObject<Movie>
    {
        public MovieAccessObject(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public IEnumerable<Movie> GetMoviesDirectedBy(string directorName)
        {
            return Transact(() => Session.QueryOver<Movie>()
                                         .Where(m => m.Director == directorName)
                                         .List());
        }

        public IEnumerable<Movie> GetMoviesWith(string actorName)
        {
            return Transact(() => Session.QueryOver<Movie>()
                                         .OrderBy(m => m.UnitPrice).Asc
                                         .Inner.JoinQueryOver<ActorRole>(m => m.Actors)
                                         .Where(a => a.Actor == actorName)
                                         .List());
        }

        public Book GetBookByISBN(string isbn)
        {
            return Transact(() => Session.QueryOver<Book>()
                                         .Where(b => b.ISBN == isbn)
                                         .SingleOrDefault());
        }

        public IEnumerable<Product> GetProductByPrice(decimal minPrice, decimal maxPrice)
        {
            return Transact(() => Session.QueryOver<Product>()
                                         .Where(p => p.UnitPrice >= minPrice
                                                     && p.UnitPrice <= maxPrice)
                                         .OrderBy(p => p.UnitPrice).Asc
                                         .List());
        }
    }
}
