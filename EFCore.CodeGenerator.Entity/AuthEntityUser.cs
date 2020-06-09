using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class AuthEntityUser
    {
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public Guid EntityId { get; set; }

        public string FilterGroupJson { get; set; }

        public bool IsLocked { get; set; }

        public DateTime CreatedTime { get; set; }

        public virtual AuthEntityInfo Entity { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
