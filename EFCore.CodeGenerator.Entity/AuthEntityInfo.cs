using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthEntityInfo
    {
        public AuthEntityInfo()
        {
            this.AuthEntityRole = new HashSet<AuthEntityRole>();
            this.AuthEntityUser = new HashSet<AuthEntityUser>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string TypeName { get; set; }

        public bool AuditEnabled { get; set; }

        public string PropertyJson { get; set; }

        public virtual ICollection<AuthEntityRole> AuthEntityRole { get; set; }

        public virtual ICollection<AuthEntityUser> AuthEntityUser { get; set; }
    }
}
