using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EagerLoading
{
    public abstract class NHMutation
    {
        protected NHMutation()
        {
            var user = String.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
            ErfasstDatum = DateTime.Now;
            ErfasstBenutzer = user;
            MutationDatum = DateTime.Now;
            MutationBenutzer = user;
        }

        public virtual DateTime ErfasstDatum { get; set; }
        public virtual string ErfasstBenutzer { get; set; }
        public virtual DateTime? MutationDatum { get; set; }
        public virtual string MutationBenutzer { get; set; }
    }
}
