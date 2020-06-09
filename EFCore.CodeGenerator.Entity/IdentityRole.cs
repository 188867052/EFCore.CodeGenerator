using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityRole
    {
        public IdentityRole()
        {
            this.AuthEntityRole = new HashSet<AuthEntityRole>();
            this.AuthModuleRole = new HashSet<AuthModuleRole>();
            this.IdentityRoleClaim = new HashSet<IdentityRoleClaim>();
            this.IdentityUserRole = new HashSet<IdentityUserRole>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public string ConcurrencyStamp { get; set; }

        public string Remark { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsDefault { get; set; }

        public bool IsSystem { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedTime { get; set; }

        public DateTime? DeletedTime { get; set; }

        public Guid? MessageId { get; set; }

        public virtual InfosMessage Message { get; set; }

        public virtual ICollection<AuthEntityRole> AuthEntityRole { get; set; }

        public virtual ICollection<AuthModuleRole> AuthModuleRole { get; set; }

        public virtual ICollection<IdentityRoleClaim> IdentityRoleClaim { get; set; }

        public virtual ICollection<IdentityUserRole> IdentityUserRole { get; set; }
    }
}
