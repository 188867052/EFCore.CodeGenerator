using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityUserLogin
    {
        public Guid Id { get; set; }

        public string LoginProvider { get; set; }

        public string ProviderKey { get; set; }

        public string ProviderDisplayName { get; set; }

        public string Avatar { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedTime { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
