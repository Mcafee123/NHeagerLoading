using EagerLoading.NHObj;
using FluentNHibernate.Automapping;

namespace itree.gef.ddb.Infrastructure.Overrides
{
    public class NHBewilligungMappingOverride : NHOverrideBase<NHBewilligung>
    {
        public override void Override(AutoMapping<NHBewilligung> mapping)
        {            
            AddRef(mapping, b => b.Dossier);
        }
    }
}