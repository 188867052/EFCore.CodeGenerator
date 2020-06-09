using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthModuleRole
    {
        public Guid Id { get; set; }

        public int ModuleId { get; set; }

        public int RoleId { get; set; }

        public virtual AuthModule Module { get; set; }

        public virtual IdentityRole Role { get; set; }
    }
}
