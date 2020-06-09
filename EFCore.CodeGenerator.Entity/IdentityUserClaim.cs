using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityUserClaim
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
