using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace itree.gef.ddb.Infrastructure.Cfg.Conventions
{
    public class TableNameConvention : ConventionBase, IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            // take the "NH" off the table name and add "T_"
            var cleanTableName = RemoveNHFromName(instance.TableName);
            var prefixedTableName = String.Format("`T_{0}`", cleanTableName);
            instance.Table(prefixedTableName);
        }

        public string RemoveNHFromName(string objectName)
        {
            if (objectName.StartsWith("`")) objectName = objectName.Substring(1);
            if (objectName.EndsWith("`")) objectName = objectName.Substring(0, objectName.Length - 1);
            var name = objectName.Replace("NH", "");
            return name;
        }
    }
}
