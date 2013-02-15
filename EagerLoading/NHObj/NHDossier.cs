using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.NHObj
{
    public class Dossier : NHIdBase
    {
        public Dossier()
        {

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Bewilligungen = new List<Bewilligung>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual IList<Bewilligung> Bewilligungen { get; set; }
    }
}
