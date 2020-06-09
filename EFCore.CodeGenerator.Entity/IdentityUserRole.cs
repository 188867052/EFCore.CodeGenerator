using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityUserRole
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public int RoleId { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? DeletedTime { get; set; }

        public virtual IdentityRole Role { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
