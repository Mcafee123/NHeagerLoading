using EagerLoading.NHObj;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace itree.gef.ddb.Infrastructure.Overrides
{
    public class NHDossierMappingOverride : NHOverrideBase<NHDossier>
    {
        public override void Override(AutoMapping<NHDossier> mapping)
        {
            //AddMany(mapping, d => d.Notizen).Cascade.AllDeleteOrphan();
            AddMany(mapping, d => d.Bewilligungen).Cascade.AllDeleteOrphan();

            //AddRef(mapping, d => d.Patient);
        }
    }
}
