using System;

namespace itree.gef.ddb.Infrastructure.Cfg.Conventions
{
    public class ConventionBase
    {
        public string GetWithPrefix(Type t, string columnName)
        {
            var dbPrefix = "";
            //if (t == typeof(NHGrund))
            //{
            //    dbPrefix = "Grd";
            //}
            //else if (t == typeof(NHInhaltTyp))
            //{
            //    dbPrefix = "Iht";
            //}
            //else if (t == typeof (NHBewilligungsInhalt))
            //{
            //    dbPrefix = "Bin";
            //}
            //else
            //{
                dbPrefix = t.Name.Substring(2, 3);
            //}
            return String.Format("{0}{1}", dbPrefix, columnName);
        }
    }
}
