using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class IdentityLoginLog
    {
        public Guid Id { get; set; }

        public string Ip { get; set; }

        public string UserAgent { get; set; }

        public DateTime? LogoutTime { get; set; }

        public DateTime CreatedTime { get; set; }

        public int UserId { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
