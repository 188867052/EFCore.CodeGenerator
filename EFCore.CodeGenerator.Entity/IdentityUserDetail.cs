using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityUserDetail
    {
        public int Id { get; set; }

        public string RegisterIp { get; set; }

        public int UserId { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
