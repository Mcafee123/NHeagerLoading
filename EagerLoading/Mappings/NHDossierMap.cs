using EagerLoading.NHObj;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.Mappings
{
    public class NHDossierMap: ClassMap<NHDossier>
    {
        public NHDossierMap()
        {
            Id(x => x.Id);
            HasMany<NHBewilligung>(x => x.Bewilligungen)
              .KeyColumn("BewDosId")
              .AsList()
              .ForeignKeyConstraintName("FK_Bewilligung_Dossier")
              .Inverse();
        }
    }
}
