using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityOrganization
    {
        public IdentityOrganization()
        {
            this.InverseParent = new HashSet<IdentityOrganization>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public int? ParentId { get; set; }

        public virtual IdentityOrganization Parent { get; set; }

        public virtual ICollection<IdentityOrganization> InverseParent { get; set; }
    }
}
