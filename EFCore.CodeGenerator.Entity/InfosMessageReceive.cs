using System;
using System.Collections.Generic;

namespace Entities
{
    public partial class InfosMessageReceive
    {
        public Guid Id { get; set; }

        public DateTime ReadDate { get; set; }

        public int NewReplyCount { get; set; }

        public DateTime CreatedTime { get; set; }

        public Guid MessageId { get; set; }

        public int UserId { get; set; }

        public virtual InfosMessage Message { get; set; }

        public virtual IdentityUser User { get; set; }
    }
}
