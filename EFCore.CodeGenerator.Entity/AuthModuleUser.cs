using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthModuleUser
    {
        public Guid Id { get; set; }

        public int ModuleId { get; set; }

        public int UserId { get; set; }

        public bool Disabled { get; set; }

        public virtual AuthModule Module { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
