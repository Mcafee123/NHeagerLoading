using EagerLoading.NHObj;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.Mappings
{
    public class DossierMap: ClassMap<Dossier>
    {
        public DossierMap()
        {
            Id(x => x.Id);
            HasMany<Bewilligung>(x => x.Bewilligungen).KeyColumn("Dossier_id");
        }
    }
}
