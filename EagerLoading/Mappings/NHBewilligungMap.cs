using EagerLoading.NHObj;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.Mappings
{
    public class BewilligungMap: ClassMap<Bewilligung>
    {
        public BewilligungMap()
        {
            Id(x => x.Id);
            Map(x => x.Bemerkung);
            Map(x => x.Start);
            Map(x => x.Ende);
            Map(x => x.GrdZusatz);
            References<Dossier>(x => x.Dossier);
        }
    }
}
