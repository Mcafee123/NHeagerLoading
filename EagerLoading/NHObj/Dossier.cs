using Iesi.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading.NHObj
{
    public class Dossier
    {
        public Dossier()
        {

            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Bewilligungen = new List<Bewilligung>();
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }
        public virtual IList<Bewilligung> Bewilligungen { get; set; }

        public virtual Guid Id { get; set; }
        public override bool Equals(object obj)
        {
            return Equals(obj as Dossier);
        }

        private static bool IsTransient(Dossier obj)
        {
            return obj != null &&
                   Equals(obj.Id, default(Guid));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(Dossier other)
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
