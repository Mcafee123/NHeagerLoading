using Iesi.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.NHObj
{
    public class NHDossier : NHIdBase
    {
        public NHDossier()
        {

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Bewilligungen = new HashedSet<NHBewilligung>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual ISet<NHBewilligung> Bewilligungen { get; set; }
    }
}
