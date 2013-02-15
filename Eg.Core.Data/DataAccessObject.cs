using System;
using Eg.FluentMappings;
using NHibernate;
using NHibernate.Context;

namespace Eg.Core.Data
{
    public class DataAccessObject<T, TId> where T : Entity<TId>
    {
        private readonly ISessionFactory _sessionFactory;

        public DataAccessObject(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        protected ISession Session
        {
            get
            {
                if (!CurrentSessionContext.HasBind(_sessionFactory))
                {
                    CurrentSessionContext.Bind(_sessionFactory.OpenSession());
                }
                return _sessionFactory.GetCurrentSession();
            }
        }

        public T Get(TId id)
        {
            return Transact(() => Session.Get<T>(id));
        }

        public T Load(TId id)
        {
            return Transact(() => Session.Load<T>(id));
        }

        public void Save(T entity)
        {
            Transact(() => Session.SaveOrUpdate(entity));
        }

        public void Delete(T entity)
        {
            Transact(() => Session.Delete(entity));
        }

        protected TResult Transact<TResult>(Func<TResult> func)
        {
            if (!Session.Transaction.IsActive)
            {
                // Wrap in transaction
                TResult result;
                using (ITransaction tx = Session.BeginTransaction())
                {
                    result = func.Invoke();
                    tx.Commit();
                }
                return result;
            }

            // Don't wrap;
            return func.Invoke();
        }

        private void Transact(Action action)
        {
            Transact(() =>
                {
                    action.Invoke();
                    return false;
                });
        }
    }

    public class DataAccessObject<T>
        : DataAccessObject<T, Guid>
        where T : Entity
    {
        public DataAccessObject(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }
    }
}