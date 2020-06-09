using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityRoleClaim
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}
