﻿using Iesi.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.NHObj
{
    public class Bewilligung : NHIdBase
    {
        public Bewilligung()
        {
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            //Zusatzbewilligungen = new HashedSet<NHBewilligung>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        //public virtual NHBewilligung Grundbewilligung { get; set; }
        //public virtual ISet<NHBewilligung> Zusatzbewilligungen { get; set; }
        public virtual Dossier Dossier { get; set; }
        public virtual string Bemerkung { get; set; }
        public virtual DateTime Start { get; set; }
        public virtual DateTime? Ende { get; set; }
        public virtual string GrdZusatz { get; set; }
    }
}
