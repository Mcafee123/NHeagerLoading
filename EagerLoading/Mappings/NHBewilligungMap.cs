using EagerLoading.NHObj;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.Mappings
{
    public class NHBewilligungMap: ClassMap<NHBewilligung>
    {
        public NHBewilligungMap()
        {
            Id(x => x.Id);
            Map(x => x.Bemerkung);
            Map(x => x.Start);
            Map(x => x.Ende);
            Map(x => x.GrdZusatz);
            References<NHDossier>(x => x.Dossier).Column("BewDosId").ForeignKey("FK_Bewilligung_Dossier");
        }
    }
}
