using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Mapping;
using itree.gef.ddb.Infrastructure.Cfg.Conventions;

namespace itree.gef.ddb.Infrastructure.Overrides
{
    public abstract class NHOverrideBase<T> : IAutoMappingOverride<T>
    {
        // get convention
        private readonly TableNameConvention _nameConvention = new TableNameConvention();

        protected OneToManyPart<TChild> AddMany<TChild>(AutoMapping<T> mapping, Expression<Func<T, IEnumerable<TChild>>> reference)
        {
            // get key-column name
            var colName = GetColumnName(typeof (TChild), typeof (T));
            // get key name
            var keyName = GetKeyName(typeof(TChild), typeof(T));
            return mapping.HasMany(reference).KeyColumn(colName).ForeignKeyConstraintName(keyName);
        }

        protected ManyToOnePart<TOther> AddRef<TOther>(AutoMapping<T> mapping, Expression<Func<T, TOther>> reference)
        {
            // get key-column name
            var colName = GetColumnName(typeof (T), typeof (TOther));
            // get key name
            var keyName = GetKeyName(typeof (T), typeof (TOther));
            return mapping.References(reference).Column(colName).ForeignKey(keyName);
        }

        private string GetKeyName(Type typeWithColumn, Type typeWithPk)
        {
            // build foreign key name
            var pkTableName = _nameConvention.RemoveNHFromName(typeWithPk.Name);
            var fkTableName = _nameConvention.RemoveNHFromName(typeWithColumn.Name);
            var keyName = String.Format("FK_{0}_{1}", fkTableName, pkTableName);
            return keyName;
        }

        private string GetColumnName(Type typeWithColumn, Type typeWithPk)
        {
            // build column name
            var prefixOfT = _nameConvention.GetWithPrefix(typeWithColumn, "");
            var prefixOfOther = _nameConvention.GetWithPrefix(typeWithPk, "");
            var colName = String.Format("{0}{1}Id", prefixOfT, prefixOfOther);
            return colName;
        }

        public abstract void Override(AutoMapping<T> mapping);
    }
}