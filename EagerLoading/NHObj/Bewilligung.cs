using Iesi.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.NHObj
{
    public class Bewilligung
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

        public virtual Guid Id { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as Bewilligung);
        }

        private static bool IsTransient(Bewilligung obj)
        {
            return obj != null &&
                   Equals(obj.Id, default(Guid));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(Bewilligung other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                Type otherType = other.GetUnproxiedType();
                Type thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                       otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(Guid)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }
    }
}
