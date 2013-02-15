using FluentNHibernate.Automapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading
{
    public class MappingCfg : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type typ)
        {
            if (typ == null) return false;
            return typ.Namespace != null && typ.Namespace.StartsWith("EagerLoading.NHObj");
        }
    }
}
