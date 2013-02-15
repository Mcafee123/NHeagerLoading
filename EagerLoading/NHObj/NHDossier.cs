using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
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
            Bewilligungen = new List<NHBewilligung>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual IList<NHBewilligung> Bewilligungen { get; set; }
    }
}
